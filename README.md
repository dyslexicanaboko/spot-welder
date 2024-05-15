# spot-welder

Formerly known as [SimpleClassCreator](https://github.com/dyslexicanaboko/simple-class-creator). It has been renamed to SpotWelder since it does far more now than just creating classes. This is a WPF application for handling the creation of mundane CRUD work. This is an all around software developer tool to help aid developers in creating mundane code quickly. I am focused on reducing tedium. I will expand this into including other developer centric tools, such as performing sanity checks.

I have been working on this project on and off for a long time. I normally change it depending on my needs, but I try my best to keep it generic. I have created very basic data layers that can be used as starter code [here](https://github.com/dyslexicanaboko/code-snippets/tree/master/Visual%20C%23/BasicDataLayers) that compliment the code that is generated.

## Features

- Solitary table or compound query to model, entity and interface
- Serialization options to and from CSV or JSON
- Static repository option: My implementation of an inline SQL ADO.Net repository
- Dapper repository option
- Taking C# code and converting it into a DTO, Model or different language.
  - This is a mostly working. It's less broken than it was. I am mostly having trouble with the UI, the code itself works just fine.
- Creating C# object declarations from a SQL statement to help support realistic Unit Testing scenarios where writing the dummy data is a chore.

## Upcoming changes

For other upcoming features check out the Issues section.

## Use case

- You were just assigned a new project and it requires that you introduce all of the scaffolding for five new tables. You just need to design the table and this project can do the rest for all five tables. This is not a regeneration of your entire solution, it is only for those five tables.
- For some reason your existing ORM is choking real bad at executing properly formed SQL, Linq is failing you here and you need to control your SQL. Generate a static repository for your target table and you are back in business. Only keep the code you need from what was generated.
- You need to create a DTO from an existing object or table and writing that code by hand is joyless, just generate it.

### Why I created this tool originally

- I had to create a data layer for a 130 column table in VB dot net. I refused to write it by hand and spent time writing the first version of this tool to do it for me. Work smart, not hard.
- In a past role, I saved my team weeks of work by automatically creating DTOs for every entity in our domain.

## What this project is not

This project is **not** meant to be a full template driven code generator. I think of it as a spot welder for getting starter code written quickly when you need to introduce new pipelines for new tables that have been introduced into your domain. If you need a full template driven solution consider using [CodeSmith Tools Generator](https://www.codesmithtools.com/product/generator). I have used this in the past for setting a code generation standard when multiple people need to work on a domain as is typical of software development. This product allows you to control all of your layers at a very granular level in any language. It does not just have to be C#, I have built SQL and JavaScript using this tool as there are no restrictions on that. I am not affiliated with or endorsed by CodeSmith Tools, I just really like their tool and I own a personal copy of it which I use for a larger project.
