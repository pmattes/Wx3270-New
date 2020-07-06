' wx3270 VBScript sample

' Create the connection to wx3270.
set worker = CreateObject("x3270is.WorkerConnection")

' Example: Query the cursor position.
cursor = worker.RunSafeArray("Query", Array("Cursor1"))
cursorSplit = Split(cursor, " ")
row = cursorSplit(0)
column = cursorSplit(1)

' Paste that text into the 3270 screen.
r = worker.RunSafeArray("String", Array("Cursor is at row " & row & ", column " & column))

' Then move the cursor back to where it was. Here are some different ways to do it.
r = worker.RunSafeArray("MoveCursor1", Array(row, column))
r = worker.RunRaw("MoveCursor1(" & row & "," & column & ")")