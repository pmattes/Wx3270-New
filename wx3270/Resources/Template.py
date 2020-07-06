# wx3270 Python script sample

# Import the wx3270 interface module, which lives in the install directory
import sys
sys.path.insert(0, '%INSTALL%')
import x3270if

# Create the connection to wx3270.
worker = x3270if.worker_connection()

# Example: Query the cursor position.
cursor = worker.run_action("Query", "Cursor1")
cursorSplit = cursor.split(' ')
row = cursorSplit[0]
column = cursorSplit[1]

# Paste that text into the 3270 screen.
worker.run_action("String", "Cursor is at row {0}, column {1}".format(row, column))

# Then move the cursor back to where it was.
worker.run_action("MoveCursor1", [row, column])
# Could also do: worker.run_action('MoveCursor1', row, column)
# Could also do: worker.run_action('MoveCursor1(' + row + ',' + column + ')')