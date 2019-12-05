# AmpParser
Parsing tooling - Script validation/inspection

This repository contains a parser to parse Sql into parse trees similar to the .Net Roslyn project.
As the parsing processs is build in layers, huge parts might be reusable for other systems.

This tooling is used at my employer for validation of Oracle operations. Partially to validate
new code, and partially to reverse engineer existing code.

The parsing code is not part of our core product, but should be easy to extend/deploy in your own
test tooling.

