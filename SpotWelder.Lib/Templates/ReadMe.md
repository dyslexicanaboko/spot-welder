# What are these `.template` files?

These files are templates and not intended to be buildable code. They are read in by this program and there are tags that are replaced. Edit them at your own risk.

## Naming convention

PascalCase description of the template, followed by the language extension and finally finished off by using the `.template` to indicate that it is a template file. This also prevents the IDE from interfering with wanting to format the file.

Examples:
 - MyCSharpClass.cs.template
 - MyTypeScriptType.ts.template
 - MyJavaScriptPrototype.js.template

## Why are there strings wrapped in curly braces?

- Those are the aforementioned tags. They look like this: `{{TagName}}`
- These tags are replaced dynamically while generating code.
- In the future, it may be possible to provide your own, but it's low on the list of priorties.
- 

