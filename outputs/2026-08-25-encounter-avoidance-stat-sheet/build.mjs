import fs from "node:fs/promises";
import { FileBlob, SpreadsheetFile } from "@oai/artifact-tool";

const sourcePath = "F:/ForkBin/GameDev/outputs/2026-08-23-forward-validation/DARWIN OR DIE STAT SHEET - updated.xlsx";
const outputPath = "F:/ForkBin/GameDev/outputs/2026-08-25-encounter-avoidance-stat-sheet/DARWIN OR DIE STAT SHEET - Encounter Avoidance Update.xlsx";

const workbook = await SpreadsheetFile.importXlsx(await FileBlob.load(sourcePath));
const statSheet = workbook.worksheets.getItem("Herbivore Stat Sheet");
const calculator = workbook.worksheets.getItem("Independent Calculator");

statSheet.getRange("A1:D19").values = [
  ["Starting Population", "SPO", "# of starting Population", "Baseline population count"],
  ["Predator-Active Herbivore Steps", "HPS", "Sum of living herbivores across every step where at least one carnivore is present", "Population-normalized opportunity for predator avoidance; HPS=0 means N/A"],
  ["Encountered Herbivore Steps", "EHS", "Total # of herbivore-step instances where that herbivore experienced at least one carnivore encounter", "Count each herbivore at most once per step; EHS cannot exceed HPS"],
  ["Encounters", "ECN", "Total # of interactions between an herbivore species and carnivores", "Raw interaction counter; multiple encounters can occur within one herbivore-step"],
  ["Preyed", "PREY", "Total # of times a carnivore killed the herbivore species", "Adverse event counter"],
  ["Starved", "STRV", "Total # of times the herbivore species starved to death", "Adverse event counter"],
  ["Mating", "MAT", "Total # of times a herbivore species attempted to mate", "Mating opportunity counter"],
  ["Births", "BIR", "Total # of successful Births", "Successful outcome counter"],
  ["Crowding", "CRWD", "Total # of Crowding Deaths", "Adverse event counter"],
  ["Final Population", "FPO", "SPO + BIR - PREY - STRV - CRWD", "Population conservation/reconciliation formula"],
  ["Inverse Preyed Average", "pAVI", "1-(PREY/ECN)", "Survival after contact; ECN=0 means N/A"],
  ["Inverse Encounter Average", "eAVI", "1-(EHS/HPS)", "Encounter avoidance; HPS=0 means N/A, while HPS>0 and EHS=0 is a valid 1"],
  ["Predation Average", "predAVG", "Average of applicable pAVI and eAVI", "APS predation component; use the one valid value when the other is N/A; both N/A is neutral"],
  ["Inverse Starved Average", "sAVI", "1-(STRV/SPO+BIR-PREY)", "Inverse exposure rate; zero exposure means N/A, with starvation penalty 0 in APS"],
  ["Inverse Crowding Average", "cAVI", "1-(CRWD/SPO+BIR-PREY-STRV)", "Inverse exposure rate; zero exposure means N/A, with crowding penalty 0 in APS"],
  ["Birth Average", "bAVG", "BIR/MAT", "Success-per-opportunity rate; MAT=0 means N/A"],
  ["Replication Fitness Score", "RFS", "(FPO-SPO)*bAVG", "Derived replication score; valid zero bAVG remains a zero multiplier; no birth opportunity is N/A"],
  [null, null, null, null],
  ["Actual Prey Score", "APS", "RFS+predAVG-(1-sAVI)-(1-cAVI)", "Composite score; predAVG replaces the lone pAVI term so avoidance and encounter survival share one 0-1 contribution"],
];
statSheet.getRange("A1:D19").format.wrapText = true;
statSheet.getRange("A1:A19").format.columnWidth = 31;
statSheet.getRange("B1:B19").format.columnWidth = 12;
statSheet.getRange("C1:C19").format.columnWidth = 70;
statSheet.getRange("D1:D19").format.columnWidth = 94;
statSheet.getRange("A1:D19").format.autofitRows();

calculator.getRange("A3:G26").clear({ applyTo: "contents" });
calculator.getRange("A4:B4").values = [["Raw simulation count", "Value"]];
calculator.getRange("A5:A14").values = [["SPO"], ["HPS"], ["EHS"], ["ECN"], ["PREY"], ["STRV"], ["MAT"], ["BIR"], ["CRWD"], ["FPO"]];
calculator.getRange("A15").values = [["Input status"]];
calculator.getRange("B15").formulas = [["=IF(COUNT(B5:B14)=10,\"READY\",\"AWAITING RAW COUNTS\")"]];
calculator.getRange("A17:G17").values = [["Metric", "Code", "Numerator / net change", "Denominator / multiplier", "Independent result", "Status", "Contract"]];
calculator.getRange("A18:B26").values = [
  ["Final Population", "FPO"], ["Inverse Preyed Average", "pAVI"], ["Inverse Encounter Average", "eAVI"],
  ["Predation Average", "predAVG"], ["Inverse Starved Average", "sAVI"], ["Inverse Crowding Average", "cAVI"],
  ["Birth Average", "bAVG"], ["Replication Fitness Score", "RFS"], ["Actual Prey Score", "APS"],
];
calculator.getRange("G18:G26").values = [
  ["SPO+BIR-PREY-STRV-CRWD; must reconcile to FPO"],
  ["1-PREY/ECN; measures survival after contact"],
  ["1-EHS/HPS; measures population-normalized encounter avoidance"],
  ["Average applicable pAVI and eAVI; one valid component stands alone; both N/A is neutral"],
  ["1-STRV/(SPO+BIR-PREY); N/A is neutral in APS"],
  ["1-CRWD/(SPO+BIR-PREY-STRV); N/A is neutral in APS"],
  ["BIR/MAT; valid zero is allowed when MAT>0"],
  ["(FPO-SPO)*bAVG"],
  ["RFS+predAVG-(1-sAVI)-(1-cAVI); N/A neutral, INVALID propagates"],
];
calculator.getRange("C18:F26").formulas = [
  ["=B14", "=B5+B12-B9-B10-B13", "=IF($B$15<>\"READY\",\"\",C18)", "=IF($B$15<>\"READY\",\"\",IF(C18=D18,\"VALID\",\"INVALID\"))"],
  ["=B9", "=B8", "=IF($B$15<>\"READY\",\"\",IF(F19=\"VALID\",1-C19/D19,\"\"))", "=IF($B$15<>\"READY\",\"\",IF(OR(C19<0,D19<0,C19>D19),\"INVALID\",IF(D19=0,IF(C19=0,\"N/A\",\"INVALID\"),\"VALID\")))"],
  ["=B7", "=B6", "=IF($B$15<>\"READY\",\"\",IF(F20=\"VALID\",1-C20/D20,\"\"))", "=IF($B$15<>\"READY\",\"\",IF(OR(C20<0,D20<0,C20>D20),\"INVALID\",IF(D20=0,IF(C20=0,\"N/A\",\"INVALID\"),\"VALID\")))"],
  ["=E19", "=E20", "=IF($B$15<>\"READY\",\"\",IF(F21<>\"VALID\",\"\",IF(AND(F19=\"VALID\",F20=\"VALID\"),AVERAGE(E19,E20),IF(F19=\"VALID\",E19,E20))))", "=IF($B$15<>\"READY\",\"\",IF(OR(F19=\"INVALID\",F20=\"INVALID\"),\"INVALID\",IF(AND(F19=\"N/A\",F20=\"N/A\"),\"N/A\",\"VALID\")))"],
  ["=B10", "=B5+B12-B9", "=IF($B$15<>\"READY\",\"\",IF(F22=\"VALID\",1-C22/D22,\"\"))", "=IF($B$15<>\"READY\",\"\",IF(OR(C22<0,D22<0,C22>D22),\"INVALID\",IF(D22=0,IF(C22=0,\"N/A\",\"INVALID\"),\"VALID\")))"],
  ["=B13", "=B5+B12-B9-B10", "=IF($B$15<>\"READY\",\"\",IF(F23=\"VALID\",1-C23/D23,\"\"))", "=IF($B$15<>\"READY\",\"\",IF(OR(C23<0,D23<0,C23>D23),\"INVALID\",IF(D23=0,IF(C23=0,\"N/A\",\"INVALID\"),\"VALID\")))"],
  ["=B12", "=B11", "=IF($B$15<>\"READY\",\"\",IF(F24=\"VALID\",C24/D24,\"\"))", "=IF($B$15<>\"READY\",\"\",IF(OR(C24<0,D24<0,C24>D24),\"INVALID\",IF(D24=0,IF(C24=0,\"N/A\",\"INVALID\"),\"VALID\")))"],
  ["=B14-B5", "=E24", "=IF($B$15<>\"READY\",\"\",IF(F25=\"VALID\",C25*D25,\"\"))", "=IF($B$15<>\"READY\",\"\",IF(F24=\"INVALID\",\"INVALID\",IF(F24=\"N/A\",\"N/A\",\"VALID\")))"],
  [null, null, "=IF($B$15<>\"READY\",\"\",IF(F26=\"VALID\",IF(F25=\"VALID\",E25,0)+IF(F21=\"VALID\",E21,0)-IF(F22=\"VALID\",1-E22,0)-IF(F23=\"VALID\",1-E23,0),\"\"))", "=IF($B$15<>\"READY\",\"\",IF(OR(F18=\"INVALID\",F19=\"INVALID\",F20=\"INVALID\",F21=\"INVALID\",F22=\"INVALID\",F23=\"INVALID\",F24=\"INVALID\",F25=\"INVALID\"),\"INVALID\",\"VALID\"))"],
];

const thinBlueBorders = { preset: "all", style: "thin", color: "#D9E2F3" };
calculator.getRange("A1:G26").format.font = { typeface: "Aptos Narrow", fontSize: 11 };
calculator.getRange("A1:G26").format.wrapText = true;
calculator.getRange("A1:G1").format = { fill: "#1F4E78", font: { bold: true, color: "#FFFFFF", typeface: "Aptos Narrow", fontSize: 11 }, borders: thinBlueBorders, wrapText: true };
calculator.getRange("A2:G2").format = { fill: "#D9EAF7", font: { italic: true, color: "#1F1F1F", typeface: "Aptos Narrow", fontSize: 11 }, borders: thinBlueBorders, wrapText: true };
calculator.getRange("A4:B4").format = { fill: "#5B9BD5", font: { bold: true, color: "#FFFFFF", typeface: "Aptos Narrow", fontSize: 11 }, borders: thinBlueBorders, wrapText: true };
calculator.getRange("A17:G17").format = { fill: "#5B9BD5", font: { bold: true, color: "#FFFFFF", typeface: "Aptos Narrow", fontSize: 11 }, borders: thinBlueBorders, wrapText: true };
calculator.getRange("A5:B15").format.borders = thinBlueBorders;
calculator.getRange("A18:G26").format.borders = thinBlueBorders;
calculator.getRange("B5:B14").format.fill = "#FFF2CC";
calculator.getRange("A15:G15").format = { fill: "#FFFFFF", font: { color: "#1F1F1F", typeface: "Aptos Narrow", fontSize: 11 }, borders: { preset: "none" }, wrapText: true };
calculator.getRange("A15:B15").format.borders = thinBlueBorders;
calculator.getRange("B15").format = { fill: "#FCE4D6", font: { bold: true, color: "#1F1F1F", typeface: "Aptos Narrow", fontSize: 11 }, borders: thinBlueBorders, wrapText: true };
calculator.getRange("C18:D25").format.numberFormat = "#,##0";
calculator.getRange("E19:E26").format.numberFormat = "0.0000";
calculator.getRange("A1:G26").format.autofitRows();

calculator.getRange("B5:B14").values = [[10], [100], [20], [25], [5], [2], [10], [5], [1], [7]];
console.log((await workbook.inspect({ kind: "table", sheetId: "Independent Calculator", range: "A15:G26", include: "values,formulas", tableMaxRows: 20, tableMaxCols: 7, maxChars: 12000 })).ndjson);
for (const testCase of [
  { name: "zero encounters with predators", inputs: [10, 100, 0, 0, 0, 2, 10, 5, 1, 12] },
  { name: "no predator activity", inputs: [10, 0, 0, 0, 0, 2, 10, 5, 1, 12] },
  { name: "invalid EHS greater than HPS", inputs: [10, 10, 11, 0, 0, 2, 10, 5, 1, 12] },
]) {
  calculator.getRange("B5:B14").values = testCase.inputs.map((value) => [value]);
  console.log(JSON.stringify({ testCase: testCase.name, results: calculator.getRange("E19:F26").values }));
}
calculator.getRange("B5:B14").clear({ applyTo: "contents" });

console.log((await workbook.inspect({ kind: "table", sheetId: "Herbivore Stat Sheet", range: "A1:D19", include: "values,formulas", tableMaxRows: 25, tableMaxCols: 4, maxChars: 12000 })).ndjson);
console.log((await workbook.inspect({ kind: "table", sheetId: "Independent Calculator", range: "A1:G26", include: "values,formulas", tableMaxRows: 30, tableMaxCols: 7, maxChars: 16000 })).ndjson);
console.log((await workbook.inspect({ kind: "match", searchTerm: "#REF!|#DIV/0!|#VALUE!|#NAME\\?|#N/A", options: { useRegex: true, maxResults: 300 }, summary: "final formula error scan" })).ndjson);

for (const sheetName of ["Herbivore Stat Sheet", "Predator Stat Sheet", "Independent Calculator"]) {
  const preview = await workbook.render({ sheetName, autoCrop: "all", scale: 1.5, format: "png" });
  await fs.writeFile(`./after-${sheetName.toLowerCase().replaceAll(" ", "-")}.png`, new Uint8Array(await preview.arrayBuffer()));
}

await (await SpreadsheetFile.exportXlsx(workbook)).save(outputPath);
console.log(JSON.stringify({ outputPath }));
