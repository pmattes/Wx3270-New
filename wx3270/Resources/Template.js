// wx3270 JScript sample

// Create the connection to wx3270.
var worker = new ActiveXObject("x3270is.WorkerConnection");

// Example: Query the cursor position.
var cursor = worker.RunJScriptArray("Query", new Array("Cursor1"));
var cursorSplit = cursor.split(" ");
var row = cursorSplit[0];
var column = cursorSplit[1];

// Paste that text into the 3270 screen.
worker.RunJScriptArray("String", new Array("Cursor is at row " + row + ", column " + column));

// Then move the cursor back to where it was. Here are some different ways to do it.
worker.RunJScriptArray("MoveCursor1", new Array(row, column));
worker.RunRaw("MoveCursor1(" + row + "," + column + ")");