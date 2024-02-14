# Excalibur

[![License](https://img.shields.io/github/license/robertcoltheart/excalibur?style=for-the-badge)](https://github.com/robertcoltheart/excalibur/blob/master/LICENSE)

A console application for creating purchase orders in Xero.

## Usage
Download the application from the releases page and put the exe into a folder. Copy any Excel files into the same folder, and run the application.

Excel files must being with a header with the following columns, with sample data shown.

| PurchaseOrderNumber | Contact | Date | DeliveryDate | Reference | Status | Line Items.AccountCode | Line Items.Description | Line Items.Quantity | Line Items.UnitAmount | Line Items.TaxType | Tracking.[Tracking category] |
| -- | -- | -- | -- | -- | -- | -- | -- | -- | -- | -- | -- |
| Used for grouping | Full name | yyyy-MM-dd | yyyy-MM-dd | Any | DRAFT | 300 | Description | 5.0 | 1.0 | INPUT | Category option |

Multiple purchase orders can be created by entering unique values for:
- PurchaseOrderNumber
- Contact
- Date
- DeliveryDate
- Reference
- Status

For example, to group line items for a single purchase order, keep the above columns identical for each line item.

You can abort the running of the app at any time by pressing `Ctrl+C`.

## Get in touch
Raise an [issue](https://github.com/robertcoltheart/excalibur/issues).

## Contributing
Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on how to contribute to this project.

## License
Excalibur is released under the [MIT License](LICENSE)
