Coding style
- This repo has strict StyleCop settings configured. Do not disable a StyleCop warning unless absolutely necessary (and always ask if this is acceptable).
- Methods that do not access instance members should be static.
- Native methods (both extern method declarations and constants) go in the NativeMethods class in NativeMethods.cs. Do not put them inline an another module.
Unit tests
- Wherever possible, new code (expecially non-display-related code) should have unit tests, added to the UnitTests project.
Commit rules
- Before committing, the C# unit tests in the UnitTest project should be run, as well as the Python unit tests in the Test folder.
