import fs from "node:fs/promises";
import { FileBlob, SpreadsheetFile } from "@oai/artifact-tool";

const sourcePath = process.argv[2]
  ?? "F:/ForkBin/GameDev/outputs/2026-08-25-herbivore-upgrades/source.xlsx";
const workbook = await SpreadsheetFile.importXlsx(await FileBlob.load(sourcePath));

const summary = await workbook.inspect({
  kind: "workbook,sheet,table",
  maxChars: 12000,
  tableMaxRows: 20,
  tableMaxCols: 10,
  tableMaxCellChars: 120,
});
console.log(summary.ndjson);

const sheets = workbook.worksheets.items;
for (const sheet of sheets) {
  const used = sheet.getUsedRange();
  const styles = await workbook.inspect({
    kind: "computedStyle",
    sheetId: sheet.name,
    range: used?.address ?? "A1:H30",
    maxChars: 5000,
  });
  console.log(`STYLE ${sheet.name}\n${styles.ndjson}`);
  const preview = await workbook.render({
    sheetName: sheet.name,
    autoCrop: "all",
    scale: 1.5,
    format: "png",
  });
  const safeName = sheet.name.replace(/[^a-z0-9]+/gi, "-").toLowerCase();
  await fs.writeFile(
    `F:/ForkBin/GameDev/outputs/2026-08-25-herbivore-upgrades/before-${safeName}.png`,
    new Uint8Array(await preview.arrayBuffer()),
  );
}
