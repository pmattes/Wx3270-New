# wx3270 PowerShell script sample

# Load the wx3270 interface DLL.
Add-Type -Path "%DLL%"

# Create the connection to wx3270.
$worker = New-Object -TypeName x3270is.WorkerConnection
#$worker.Debug = 1

# Example: Query the cursor position.
$cursor = $worker.Run("Query", "Cursor1")
$cursorSplit = $cursor.Split(" ")
$row = $cursorSplit[0]
$column = $cursorSplit[1]

# Paste that text into the 3270 screen.
$worker.Run("String", [string]::Format("Cursor is at row {0}, column {1}", $row, $column))

# Then move the cursor back to where it was. Here are some different ways to call Run to do it.
#  Variable number of arguments
$worker.Run("MoveCursor1", $row, $column)
#  Raw (do your own quoting)
$worker.RunRaw("MoveCursor1(" + $row + "," + $column + ")")
#  Untyped array
$a = ($row, $column)
$worker.Run("MoveCursor1", $a)
#  String array
$b = [string []]($row, $column)
$worker.Run("MoveCursor1", $b)