// Render the report projection; all observation rules live in worksheet.py.
import fs from 'node:fs/promises';
import { constants } from 'node:fs';
import { randomUUID } from 'node:crypto';
import os from 'node:os';
import path from 'node:path';
import { createRequire } from 'node:module';
import { pathToFileURL } from 'node:url';

const [input, output, artifactRoot, preview] = process.argv.slice(2);
if (!input || !output || !artifactRoot) {
  throw new Error('Usage: node worksheet.mjs <projection.json> <new.xlsx> <node_modules> [preview.png]');
}
const require = createRequire(path.join(path.resolve(artifactRoot), '..', 'package.json'));
const { Workbook, SpreadsheetFile } = await import(pathToFileURL(require.resolve('@oai/artifact-tool')).href);
const data = JSON.parse(await fs.readFile(input, 'utf8'));
if (data.schemaVersion !== 3) throw new Error('Unsupported worksheet projection. Re-run the current worksheet preparation tool.');
const workbook = Workbook.create();
const summary = workbook.worksheets.add('Batch summary');
const review = workbook.worksheets.add('Run review');
review.showGridLines = false;
const literal = value => typeof value === 'string' && value.startsWith('=') ? "'" + value : value;
const write = (sheet, row, values) => {
  sheet.getRangeByIndexes(row, 0, values.length, values[0].length).values = values.map(r => r.map(literal));
};
// Calculations and source interpretation stay in worksheet.py; this file owns presentation.
summary.showGridLines = false;
write(summary, 0, [['Simulation batch summary']]);
write(summary, 1, [[`${data.totalRuns.toLocaleString('en-US')} matching runs across ${data.groups.length} conditions. Run review and Run data contain the same ${data.exportedRows} examples.`]]);
let contextRow = 3;
for (const experiment of data.experiments) {
  const lines = [
    `Experiment: ${experiment.name}. Owner: ${experiment.owner}.`,
    `Question: ${experiment.question}`,
    `Starting Hares / Foxes / Plants: ${experiment.populations}. Map: ${experiment.map}.`,
    `Seeds: ${experiment.seeds}. ${experiment.timing}.`,
    `Automatic strategies: ${experiment.strategies}. Purchase policies: ${experiment.policies}.`,
    `Stat overrides: ${experiment.overrides}.`,
  ];
  for (const line of lines) {
    summary.getRange(`A${contextRow + 1}:J${contextRow + 1}`).merge();
    write(summary, contextRow, [[line]]);
    summary.getRange(`A${contextRow + 1}:J${contextRow + 1}`).format.wrapText = true;
    const linesNeeded = line.split('\n').reduce((count, part) => count + Math.max(1, Math.ceil(part.length / 160)), 0);
    summary.getRange(`A${contextRow + 1}:J${contextRow + 1}`).format.rowHeight = Math.max(22, linesNeeded * 16 + 6);
    contextRow++;
  }
  contextRow++;
}
summary.getRange(`A${contextRow + 1}:J${contextRow + 1}`).merge();
write(summary, contextRow++, [['Survival means alive at the stated horizon. Reaching it can include extinction on the final tick. Rates use all matching runs; missing required data says Not recorded.']]);
summary.getRange(`A${contextRow}:J${contextRow}`).format.rowHeight = 30;
summary.getRange(`A${contextRow}:J${contextRow}`).format.wrapText = true;
const headerRow = contextRow + 2;
const summaryHeaders = ['Condition', 'Runs', 'Horizon tick', 'Hares alive', 'Hare survival', 'All three alive',
  'All-three survival', 'Reached horizon', 'Hare extinction endpoints', 'Missing endpoint data',
  'Starting Hares / Foxes / Plants', 'Map', 'Stat overrides', 'Condition ID', 'Source report', 'Batch identity'];
write(summary, headerRow - 1, [summaryHeaders, ...data.summaryRows]);
const summaryEnd = headerRow + data.summaryRows.length;
summary.getRange(`A1:P${summaryEnd}`).format.font = {name: 'Arial', size: 11};
summary.getRange('A1').format.font = {name: 'Arial', size: 16, bold: true};
summary.getRange('A2:J2').merge();
summary.getRange('A2').format.font.size = 10;
summary.getRange(`A${headerRow}:P${summaryEnd}`).format.wrapText = true;
summary.getRange(`A${headerRow}:P${summaryEnd}`).format.verticalAlignment = 'center';
summary.getRange(`A${headerRow}:P${headerRow}`).format = {
  fill: '#344C31', font: {color: '#FFFFFF', bold: true, name: 'Arial', size: 11},
  wrapText: true, rowHeight: 46, horizontalAlignment: 'center', verticalAlignment: 'center',
};
[38, 10, 13, 14, 14, 15, 16, 16, 18, 18, 29, 26, 46, 35, 65, 40].forEach((width, index) => {
  summary.getRangeByIndexes(headerRow - 1, index, data.summaryRows.length + 1, 1).format.columnWidth = width;
});
summary.getRange(`B${headerRow + 1}:J${summaryEnd}`).setNumberFormat('#,##0');
for (const col of ['E', 'G']) summary.getRange(`${col}${headerRow + 1}:${col}${summaryEnd}`).setNumberFormat('0.0%');
summary.tables.add(`A${headerRow}:P${summaryEnd}`, true, 'BatchSummary');
data.summaryRows.forEach((row, index) => {
  const number = headerRow + index + 1;
  const lines = Math.max(1, Math.ceil(String(row[0]).length / 35));
  summary.getRange(`A${number}:P${number}`).format.rowHeight = Math.max(32, lines * 15 + 8);
  if (index % 2 === 0) summary.getRange(`A${number}:P${number}`).format.fill = '#F2F5EE';
});
summary.freezePanes.freezeRows(headerRow);
summary.freezePanes.freezeColumns(1);
write(review, 0, [['Simulation run review']]);
write(review, 1, [[`${data.exportedRows} rows shown / ${data.totalRuns.toLocaleString('en-US')} matching runs. ${data.selection}.`]]);
write(review, 2, [[`Population order: Hares / Foxes / Plants. ${data.detailedObservations ? 'Detailed' : 'Concise'} observations. Batch summary covers every matching run; human notes are yours to fill in.`]]);
write(review, 4, [data.headers, ...data.rows]);
const last = data.rows.length + 5;
review.getRange(`A1:I${last}`).format.font.name = 'Arial';
review.getRange(`A1:I${last}`).format.font.size = 11;
review.getRange('A1').format.font.size = 16;
review.getRange('A1').format.font.bold = true;
review.getRange('A2:I3').format.rowHeight = 20;
review.getRange('A2:I3').format.font.size = 10;
const widths = [31, 23, 23, 25, 57, 57, 62, 46, 42];
widths.forEach((width, index) => {
  review.getRangeByIndexes(4, index, data.rows.length + 1, 1).format.columnWidth = width;
});
review.getRange(`A5:I${last}`).format.wrapText = true;
review.getRange(`A5:I${last}`).format.verticalAlignment = 'top';
review.getRange('A5:I5').format = {
  fill: '#344C31', font: { color: '#FFFFFF', bold: true, name: 'Arial', size: 11 },
  wrapText: true, rowHeight: 60, verticalAlignment: 'center', horizontalAlignment: 'center',
};
const table = review.tables.add(`A5:I${last}`, true, 'RunReview');
table.showFilterButton = true;
for (let index = 0; index < data.rows.length; index++) {
  const row = index + 6;
  const values = data.rows[index];
  const lines = Math.max(...values.map((value, column) => String(value).split('\n')
    .reduce((n, line) => n + Math.max(1, Math.ceil(line.length / (widths[column] * 0.95))), 0)));
  review.getRange(`A${row}:I${row}`).format.rowHeight = Math.min(409, Math.max(90, lines * 15 + 12));
  if (index % 2 === 0) review.getRange(`A${row}:I${row}`).format.fill = '#F2F5EE';
  review.getRange(`H${row}`).format.fill = '#FFF2CC';
  review.getRange(`I${row}`).format.font.color = '#1F4E78';
}
review.freezePanes.freezeRows(5);
review.freezePanes.freezeColumns(1);

const flat = workbook.worksheets.add('Run data');
flat.showGridLines = false;
const columns = data.runData.headers.length;
const flatEnd = data.runData.rows.length + 1;
write(flat, 0, [data.runData.headers, ...data.runData.rows]);
const flatRange = flat.getRangeByIndexes(0, 0, flatEnd, columns);
flatRange.format.font = {name: 'Arial', size: 10};
flatRange.format.rowHeight = 22;
flat.getRangeByIndexes(0, 0, 1, columns).format = {
  fill: '#344C31', font: {color: '#FFFFFF', bold: true, name: 'Arial', size: 10},
  wrapText: true, rowHeight: 76, horizontalAlignment: 'center', verticalAlignment: 'center',
};
data.runData.headers.forEach((header, index) => {
  flat.getRangeByIndexes(0, index, flatEnd, 1).format.columnWidth =
    ['Condition', 'Source report', 'Batch identity', 'Source hash'].includes(header) ? 38 : Math.max(15, Math.min(27, header.length * 0.6));
  flat.getRangeByIndexes(1, index, flatEnd - 1, 1).setNumberFormat(data.runData.formats[index]);
});
flat.tables.add(flatRange, true, 'RunData');
flat.freezePanes.freezeRows(1);
flat.freezePanes.freezeColumns(2);

workbook.recalculate();
if (preview) {
  const blob = await workbook.render({ sheetName: 'Batch summary', range: `A1:J${summaryEnd}`, scale: 1, format: 'png' });
  await fs.writeFile(preview, new Uint8Array(await blob.arrayBuffer()));
}
const result = await SpreadsheetFile.exportXlsx(workbook);
// Exclusive creation protects both pre-existing workbooks and concurrent exports.
const temporary = path.join(os.tmpdir(), `cellsim-workbook-${randomUUID()}.xlsx`);
try {
  await result.save(temporary);
  await fs.copyFile(temporary, output, constants.COPYFILE_EXCL);
} finally {
  await fs.rm(temporary, { force: true });
}
console.log(`Created ${output}`);
