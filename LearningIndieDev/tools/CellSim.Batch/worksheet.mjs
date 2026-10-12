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
if (data.schemaVersion !== 4) throw new Error('Unsupported worksheet projection. Re-run the current worksheet preparation tool.');
const workbook = Workbook.create();
console.log(`Building Excel workbook: ${data.summaryTable.rows.length.toLocaleString('en-US')} conditions, ${data.choiceImpact.rows.length.toLocaleString('en-US')} comparisons, ${data.exportedRows.toLocaleString('en-US')} review runs.`);
const summary = workbook.worksheets.add('Batch summary');
const impact = workbook.worksheets.add('Choice impact');
const review = workbook.worksheets.add('Run review');
review.showGridLines = false;
const literal = value => typeof value === 'string' && value.startsWith('=') ? "'" + value : value;
const write = (sheet, row, values) => {
  sheet.getRangeByIndexes(row, 0, values.length, values[0].length).values = values.map(r => r.map(literal));
};
const renderTable = (sheet, table, start, name) => {
  const columns = table.headers.length, count = table.rows.length;
  if (!columns || !count) return start;
  write(sheet, start, [table.headers, ...table.rows]);
  console.log(`Formatting ${sheet.name} table ...`);
  const range = sheet.getRangeByIndexes(start, 0, count + 1, columns);
  range.format.font = {name: 'Arial', size: 11};
  range.format.verticalAlignment = 'center';
  range.format.rowHeight = 34;
  sheet.getRangeByIndexes(start, 0, 1, columns).format = {
    fill: '#344C31', font: {color: '#FFFFFF', bold: true, name: 'Arial', size: 11},
    wrapText: true, rowHeight: 76, horizontalAlignment: 'center', verticalAlignment: 'center',
  };
  table.headers.forEach((header, index) => {
    const descriptive = /condition|source|map|stat overrides|identity|starting|status/i.test(header);
    sheet.getRangeByIndexes(start, index, 1, 1).format.columnWidth = descriptive ? 38 : Math.max(14, Math.min(24, header.length * 0.65));
    sheet.getRangeByIndexes(start + 1, index, count, 1).setNumberFormat(/rate|95%/.test(header) ? '0.0%' : /^(Mean|Median)/.test(header) ? '0.0' : table.formats[index]);
    if (descriptive) sheet.getRangeByIndexes(start + 1, index, count, 1).format.wrapText = true;
  });
  const heights = table.rows.map(row => {
    const lines = Math.max(1, ...row.slice(0, 3).map(value => Math.ceil(String(value ?? '').length / 35)));
    return Math.max(34, lines * 15 + 8);
  });
  // Row heights apply to the whole row; batch adjacent equal heights to avoid thousands of format edits.
  for (let index = 0; index < count;) {
    const first = index, height = heights[index++];
    while (index < count && heights[index] === height) index++;
    sheet.getRangeByIndexes(start + first + 1, 0, index - first, 1).format.rowHeight = height;
  }
  sheet.tables.add(range, true, name);
  sheet.freezePanes.freezeRows(start + 1);
  sheet.freezePanes.freezeColumns(1);
  return start + count + 1;
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
for (const note of [
  `Export goal: at least ${data.goals[0]} Hares, ${data.goals[1]} Foxes and ${data.goals[2]} Plants at each condition's final horizon. These review criteria do not change the simulation.`,
  'Counts and rates cover every matching run. Unknown goal outcomes leave the rate blank. Endpoint averages use actual final ticks, including early extinctions.',
  '95% Wilson intervals assume independent seed trials within each condition. A two-seed screen has wide uncertainty. Shared seeds across conditions are paired observations.',
  'Sorted by the goal success lower bound, then run count. Promising results are discovery candidates; confirmation needs new seeds. No-purchase success covers only the recorded horizon.',
]) {
  summary.getRange(`A${contextRow + 1}:J${contextRow + 1}`).merge();
  write(summary, contextRow++, [[note]]);
  summary.getRange(`A${contextRow}:J${contextRow}`).format.rowHeight = 34;
  summary.getRange(`A${contextRow}:J${contextRow}`).format.wrapText = true;
}
const headerRow = contextRow + 2;
const summaryEnd = renderTable(summary, data.summaryTable, headerRow - 1, 'BatchSummary');
console.log('Batch summary built. Building same-seed choice comparisons ...');
summary.getRange(`A1:J${headerRow - 1}`).format.font = {name: 'Arial', size: 11};
summary.getRange('A1').format.font = {name: 'Arial', size: 16, bold: true};
summary.getRange('A2:J2').merge();
summary.getRange('A2').format.font.size = 10;
impact.showGridLines = false;
impact.getRange('A1:J6').format.columnWidth = 18;
write(impact, 0, [['Same-seed choice comparisons']]);
impact.getRange('A1').format.font = {name: 'Arial', size: 16, bold: true};
[
  'Mutations compare with Skip all Mutations at the same purchase policy. Purchases compare with No purchases at the same Mutation strategy. Other executed settings and source batch must match.',
  'Controls may be outside the condition text filter; the same seed filter applies to both arms. Wins/losses compare the export goal. Missing controls or outcomes remain visible.',
  'These are automated-policy outcomes, not human playtest evidence. Paired differences describe the observed seeds; no significance test or proof of enjoyable player agency is claimed.',
].forEach((note, index) => {
  impact.getRangeByIndexes(index + 1, 0, 1, 10).merge();
  write(impact, index + 1, [[note]]);
  impact.getRangeByIndexes(index + 1, 0, 1, 10).format = {font: {name: 'Arial', size: 11}, wrapText: true, rowHeight: 36};
});
renderTable(impact, data.choiceImpact, 5, 'ChoiceImpact');
console.log('Choice comparisons built. Building readable run review ...');
if (!data.choiceImpact.rows.length) {
  impact.getRange('A6:J6').merge();
  write(impact, 5, [['Only control conditions selected. There is no Mutation or purchase comparison to make.']]);
  impact.getRange('A6:J6').format = {font: {name: 'Arial', size: 11}, wrapText: true, rowHeight: 34};
}
write(review, 0, [['Simulation run review']]);
write(review, 1, [[`${data.exportedRows} rows shown / ${data.totalRuns.toLocaleString('en-US')} matching runs. ${data.selection}.`]]);
write(review, 2, [[data.selectionMode === 'promising' ? 'Shortlist order: goal met, no-purchase goal met, endpoint tick, final Hares / Foxes / Plants, then lowest seed. Use Batch summary for rates.' : `Population order: Hares / Foxes / Plants. ${data.detailedObservations ? 'Detailed' : 'Concise'} observations. Batch summary covers every matching run.`]]);
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
  review.getRangeByIndexes(4, index, 1, 1).format.columnWidth = width;
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
  review.getRange(`A${row}`).format.rowHeight = Math.min(409, Math.max(90, lines * 15 + 12));
  if (index % 2 === 0) review.getRange(`A${row}:I${row}`).format.fill = '#F2F5EE';
  review.getRange(`H${row}`).format.fill = '#FFF2CC';
  review.getRange(`I${row}`).format.font.color = '#1F4E78';
}
review.freezePanes.freezeRows(5);
review.freezePanes.freezeColumns(1);

const flat = workbook.worksheets.add('Run data');
console.log('Run review built. Building pivot-ready run data ...');
flat.showGridLines = false;
const columns = data.runData.headers.length;
const flatEnd = data.runData.rows.length + 1;
write(flat, 0, [data.runData.headers, ...data.runData.rows]);
const flatRange = flat.getRangeByIndexes(0, 0, flatEnd, columns);
flatRange.format.font = {name: 'Arial', size: 10};
flat.getRangeByIndexes(0, 0, flatEnd, 1).format.rowHeight = 22;
flat.getRangeByIndexes(0, 0, 1, columns).format = {
  fill: '#344C31', font: {color: '#FFFFFF', bold: true, name: 'Arial', size: 10},
  wrapText: true, rowHeight: 76, horizontalAlignment: 'center', verticalAlignment: 'center',
};
data.runData.headers.forEach((header, index) => {
  flat.getRangeByIndexes(0, index, 1, 1).format.columnWidth =
    ['Condition', 'Source report', 'Batch identity', 'Source hash'].includes(header) ? 38 : Math.max(15, Math.min(27, header.length * 0.6));
  flat.getRangeByIndexes(1, index, flatEnd - 1, 1).setNumberFormat(data.runData.formats[index]);
});
flat.tables.add(flatRange, true, 'RunData');
flat.freezePanes.freezeRows(1);
flat.freezePanes.freezeColumns(2);

workbook.recalculate();
console.log('Workbook tables built. Preparing Excel file ...');
if (preview) {
  await fs.writeFile(preview.replace(/\.png$/i, '-inspection.json'), JSON.stringify(await workbook.inspect({kind: 'table', range: `Batch summary!A${headerRow}:J${Math.min(summaryEnd, headerRow + 5)}`, include: 'values,formulas', tableMaxRows: 6, tableMaxCols: 10, maxChars: 3500})));
  const blob = await workbook.render({ sheetName: 'Batch summary', range: `A1:J${Math.min(summaryEnd, headerRow + 8)}`, scale: 1, format: 'png' });
  await fs.writeFile(preview, new Uint8Array(await blob.arrayBuffer()));
  for (const [sheetName, range, suffix] of [['Choice impact', 'A1:J12', 'choices'], ['Run review', 'A1:I9', 'review'], ['Run data', 'A1:L7', 'data']]) {
    const image = await workbook.render({sheetName, range, scale: 1, format: 'png'});
    await fs.writeFile(preview.replace(/\.png$/i, `-${suffix}.png`), new Uint8Array(await image.arrayBuffer()));
  }
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
