using Excalibur.Workflows;

var workflow = new PurchaseOrderWorkflow();
await workflow.ProcessPurchaseOrder();

Console.WriteLine("Completed process workflow");
