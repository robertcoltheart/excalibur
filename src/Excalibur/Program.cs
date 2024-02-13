using Excalibur.Input;
using Excalibur.Workflows;

var reader = new ExcelReader();
var workflow = new PurchaseOrderWorkflow(reader);

await workflow.Process();
