# Order Reader

Order Reader is a desktop application made to speed up the process of inputting order data received in PDF or Excel files into an ERP system. It does that by reading the text off of those order files, extracting the important information about the orders, like customers, dates, reference numbers, and products along with quantities, and it generates a CSV file which can then be read by some CSV systems.

## Features

* **Ease of use:** Order Reader is very simple to use. Once you have all the customer and product information in it, the normal daily use is to: 1. Open the app -> 2. Drag and Drop the order files on the orders panel -> 3. Press the **Process** button.
* **Automatic updates:** Order Reader automatically downloads the updates when they are available. It will then show you a notification in the bottom right corner of the screen to let you know that you can install the update. If you want to install the update straight away, you can press "Restart Now" button. Otherwise, the update will be installed the next time you launch Order Reader.

## Usage case

The application has been made with a specific use case in mind (in the company that I work for) but may be used by anyone who also has a similar setup.

Currently, the code that extracts the information from the order files, and generates the CSV files can only be edited/expanded by editing the project, but in the future, I'm planning to include some dev tools like built in Lua interpreter that will let anyone write their own custom parsing logic, without having to edit and recompile the application itself.

## Supported file types

Currently the application can read information from **PDF** and **Excel** files. It does that by simply extracting all the text content from those files and then going through them line-by-line to look for relevant data. In the case of **Excel**, each line of text represents a single cell, and the first two lines represent the number of columns and rows that are being read.

As far as exporting goes, it can export **CSV** files which some ERP systems can read from, and it is also able to export/print a **PDF** file with a table that makes the orders more clear.
