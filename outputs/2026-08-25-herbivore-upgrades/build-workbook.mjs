import fs from "node:fs/promises";
import { FileBlob, SpreadsheetFile } from "@oai/artifact-tool";

const workDir = "F:/ForkBin/GameDev/outputs/2026-08-25-herbivore-upgrades";
const inputPath = "F:/ForkBin/GameDev/outputs/2026-08-25-encounter-avoidance-stat-sheet/DARWIN OR DIE STAT SHEET - Encounter Avoidance Update.xlsx";
const outputPath = `${workDir}/DARWIN OR DIE STAT SHEET - Herbivore Upgrades.xlsx`;

const workbook = await SpreadsheetFile.importXlsx(await FileBlob.load(inputPath));
const sheet = workbook.worksheets.add("Herbivore Upgrades");
sheet.showGridLines = false;

sheet.mergeCells("A1:G1");
sheet.getRange("A1").values = [["BEV Experimental Herbivore Upgrade Paths"]];
sheet.mergeCells("A2:G2");
sheet.getRange("A2").values = [[
  "Each path changes one primary survival mechanism. Values below are implemented starting rules and remain subject to controlled balance testing.",
]];

sheet.getRange("A4:G8").values = [
  ["Upgrade", "Upgrade ID", "Per-Level Rule Change", "Resolver Rule", "Primary Slash-Line Target", "Expected Outcome", "Secondary Effect / Tradeoff"],
  ["Tough Hide", "tough-hide", "+2 BlockAmount", "Effective Block = Base Block + (Level x 2)", "pAVI", "Fewer PREY deaths per ECN; better survival after predator contact.", "Does not reduce ECN. Longer survival can create later starvation or crowding pressure."],
  ["Efficient Digestion", "efficient-digestion", "+1 Energy gained per successful feeding; plant consumption is unchanged", "Energy Gain = Food Energy Value + Level, capped by Maximum Energy", "sAVI and RFS", "Fewer STRV deaths and more energy available for reproduction.", "Additional births can increase CRWD. This does not reduce Metabolism."],
  ["Crowding Tolerance", "crowding-tolerance", "+1 crowding-only tolerated group member", "Crowding Excess = MAX(0, Group Size - Base Crowding Threshold - Level)", "cAVI", "Crowding energy penalties and CRWD begin at a denser group size.", "Mating and movement group caps remain unchanged; stress resumes above the expanded threshold."],
  ["Escape Artist", "escape-artist", "+0.5 Movement Speed while Fleeing only", "Flee Speed = Base Movement Speed + (Level x 0.5)", "eAVI", "Fewer future EHS relative to HPS by opening distance from predators faster.", "Attacks resolve before movement, so it cannot cancel the current ECN. Fleeing can delay feeding or mating."],
];

sheet.mergeCells("A11:G11");
sheet.getRange("A11").values = [["Two-of-Four Reward Offer Rules"]];
sheet.getRange("A12:C17").values = [
  ["Stage", "Implemented Rule", "Design Purpose"],
  ["Scope", "The four paths replace Faster / Attack / Block only when BEV Experimental Features is enabled for a herbivore player.", "Keeps default gameplay unchanged."],
  ["First reward", "Show two distinct paths selected deterministically from the run seed.", "Provides variation without presenting all four choices at once."],
  ["Later rewards", "Keep the previously chosen path in slot one; cycle slot two through the other three paths.", "Allows deliberate leveling while still presenting alternatives."],
  ["Level tracking", "Every successful purchase adds one level to that upgrade ID and reapplies its per-level rule change.", "Makes cumulative build strength visible on the reward and results screens."],
  ["Replay behavior", "The same run seed, continuing path, and offer rotation produce the same pair.", "Preserves deterministic testing and replay expectations."],
];

const darkBlue = "#1F4E78";
const mediumBlue = "#5B9BD5";
const lightBlue = "#D9EAF7";
const lightBorder = "#D9E2F3";
const paleYellow = "#FFF2CC";
const bodyFont = { name: "Aptos Narrow", size: 11, color: "#1F1F1F" };

sheet.getRange("A1:G1").format = {
  fill: darkBlue,
  font: { ...bodyFont, bold: true, color: "#FFFFFF", size: 16 },
  verticalAlignment: "center",
};
sheet.getRange("A2:G2").format = {
  fill: lightBlue,
  font: { ...bodyFont, italic: true },
  wrapText: true,
  verticalAlignment: "center",
};
sheet.getRange("A4:G4").format = {
  fill: mediumBlue,
  font: { ...bodyFont, bold: true, color: "#FFFFFF" },
  wrapText: true,
  verticalAlignment: "center",
};
sheet.getRange("A5:G8").format = {
  font: bodyFont,
  wrapText: true,
  verticalAlignment: "top",
};
sheet.getRange("E5:E8").format.fill = paleYellow;
sheet.getRange("A11:G11").format = {
  fill: darkBlue,
  font: { ...bodyFont, bold: true, color: "#FFFFFF", size: 13 },
  verticalAlignment: "center",
};
sheet.getRange("A12:C12").format = {
  fill: mediumBlue,
  font: { ...bodyFont, bold: true, color: "#FFFFFF" },
  wrapText: true,
  verticalAlignment: "center",
};
sheet.getRange("A13:C17").format = {
  font: bodyFont,
  wrapText: true,
  verticalAlignment: "top",
};

for (const address of ["A4:G8", "A12:C17"]) {
  sheet.getRange(address).format.borders = {
    preset: "all",
    style: "thin",
    color: lightBorder,
  };
}

sheet.getRange("A1:G1").format.rowHeight = 28;
sheet.getRange("A2:G2").format.rowHeight = 34;
sheet.getRange("A4:G4").format.rowHeight = 38;
sheet.getRange("A5:G8").format.rowHeight = 62;
sheet.getRange("A11:G11").format.rowHeight = 26;
sheet.getRange("A12:C12").format.rowHeight = 34;
sheet.getRange("A13:C17").format.rowHeight = 62;
sheet.getRange("A13:C13").format.rowHeight = 82;

sheet.getRange("A:A").format.columnWidth = 24;
sheet.getRange("B:B").format.columnWidth = 27;
sheet.getRange("C:C").format.columnWidth = 42;
sheet.getRange("D:D").format.columnWidth = 48;
sheet.getRange("E:E").format.columnWidth = 23;
sheet.getRange("F:F").format.columnWidth = 43;
sheet.getRange("G:G").format.columnWidth = 48;
sheet.freezePanes.freezeRows(4);

const upgradeTable = sheet.tables.add("A4:G8", true, "HerbivoreUpgradePathsTable");
upgradeTable.style = "TableStyleMedium2";
upgradeTable.showFilterButton = false;
const offerTable = sheet.tables.add("A12:C17", true, "HerbivoreUpgradeOfferRulesTable");
offerTable.style = "TableStyleMedium2";
offerTable.showFilterButton = false;

const inspection = await workbook.inspect({
  kind: "table",
  range: "Herbivore Upgrades!A1:G17",
  include: "values,formulas",
  tableMaxRows: 20,
  tableMaxCols: 8,
  maxChars: 12000,
});
console.log(inspection.ndjson);

const errors = await workbook.inspect({
  kind: "match",
  searchTerm: "#REF!|#DIV/0!|#VALUE!|#NAME\\?",
  options: { useRegex: true, maxResults: 300 },
  summary: "final formula error scan",
});
console.log(errors.ndjson);

for (const current of workbook.worksheets.items) {
  const preview = await workbook.render({
    sheetName: current.name,
    autoCrop: "all",
    scale: 1.5,
    format: "png",
  });
  const safeName = current.name.replace(/[^a-z0-9]+/gi, "-").toLowerCase();
  await fs.writeFile(
    `${workDir}/after-${safeName}.png`,
    new Uint8Array(await preview.arrayBuffer()),
  );
}

const output = await SpreadsheetFile.exportXlsx(workbook);
await output.save(outputPath);
console.log(outputPath);
