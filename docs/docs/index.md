---
title: About
sidebar_position: 1
---

Proof of concept of library system for education purpose that i use to explain in my [telegram channel about .NET](https://t.me/sh_dotnet)

[Event storming Miro board](https://miro.com/app/board/uXjVKddnAyU=/)

[Entity relationship diagram](./erd)

## Domain
There are books, that have one or more authors, title and a cover. Books uniquely identified by ISBN and publication date.

Library has limited count of books. There must be accounting of books.

Every book has a history of borrowing (who and when borrowed and when returned) Every abonent also has a same history (which books were borrowed and when returned)

On the site we can see book cover, authors, title. How many times book was borrowed, and how many books are available. When there no available book, user can subscribe for a book availability.

If people what to borrow books, they must register them as abonents by filling form with name, address, phone number and email. Abonent can't borrow more than 5 books at the same time. Can't borrow books for more than 1 month.

#### What else we can do:

Tags for categories (language of book, country, theme etc)  
Notify when book borrowing time has expired. If so, we can charge a fine (e.g. 5% of book price per day)  
Every book has price and amortization percent that we can track. Also, we can charge a fine, if book has been spoiled  
Book recommendation system