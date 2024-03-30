# BookLibrary
Proof of concept of library system for education purpose

[Event storming Miro board](https://miro.com/app/board/uXjVKddnAyU=/)

### Domain

There are books, that have one or more authors, title and a cover.
Books uniquely identified by ISBN and publication date.

Library has limited count of books. There must be accounting of books.

Every book has a history of borrowing (who and when borrowed and when returned)
Every abonent also has a same history (which books were borrowed and when returned)

On the site we can see book cover, authors, title. How many times book was borrowed, and how many books are available.
When there no available book, user can subscribe for a book availablity.

If people what to borrow books, they must register them as abonents by filling form with name, address, phone number and email.
Abonent can't borrow more than 5 books at the same time. Can't borrow books for more than 1 month.

What else we can do:

- Tags for categories (language of book, country, theme etc)
- Notify when book borrowing time has expired. If so, we can charge a fine (e.g. 5% of book price per day)
- Every book has price and amortization percent that we can track. Also, we can charge a fine, if book has been spoiled
- Book recomendation system

# Same in russian:

### Предметная область

Есть книги, выпущенные некоторыми определенными авторами.
Выпуски книг уникально идентифицируются по ISBN и дате публикации.

У каждой книги есть один или более авторов, заголовок, обложка.
Количество книг в библиотеке ограничено, должен быть их учет. 

У каждой книги должна быть история хождения книг (когда и кто взял, когда вернул).
У каждого абонемента должна быть история книг (когда взял, когда вернул).

На сайте должно быть видно обложку книги, список авторов, сколько книг в наличии и счетчик сколько раз брали данную книгу.
Если книг в наличии нет, то показывать когда она освободится (вернут в ближайшее время).

Абонемент может оформить кто угодно. В качестве реквизитов абонента указываются: ФИО, адрес места жительства, номер мобильного телефона и электронная почта.

У одного абонента не может быть на руках более 5 книг. Нельзя брать книги более чем на 1 месяц.

## Дополнительно можно сделать:

Для удобства категоризации, книгам могут быть проставлены некоторые теги (например язык, страна, тематика).

За два дня до просрочки происходит рассылка писем или смс, каждый день в 12 часов по времени абонента.
В случае просрочки сдачи книг, абоненту начисляется штраф, в размере 5% от стоимости книги за каждый день просрочки.

Каждая книга имеет цену и уровень амортизации. Уровень амортизации оценивает библиотекарь при приёме книги и устанавливает его по факту.
Если книга пришла в негодность более чем на 20%, накладывается штраф на абонента в размере 20% стоимости от книги.

Рекомендательная система с рассылкой:
Абонентам нужно делать рассылку-предложения книг, на основе их интересов. Например вышла новая книга автора которого он брал и держал дольше двух дней.
Или если вышла книга подходящая под категории уже ранее взятых книг. В рассылке можно писать прислать карточку самой книги и кнопка побуждающая на онлайн-бронь).

Реализовать фейковую-онлайн оплату абонемента, заказ доставки книги до дома. Возможность выкупить книгу через книжные магазины партнёры библиотеки.

