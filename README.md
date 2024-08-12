<h1 align="center">Программа учета и обработки заказа</h1>

<h3 align="center">Описание</h3>
<p align="center">
Это десктопное приложение предназначено для учета и обработки заказов. Оно позволяет управлять заказами, отслеживать их статус, а также генерировать отчеты.
</p>

<h2 align="center">Функциональные возможности</h2>
<p align="center">
- Управление заказами<br>
- Отслеживание статуса заказов<br>
- Генерация отчетов<br>
- Интеграция с базами данных<br>
- Управление сотрудниками<br>
- Управление клиентами<br>
- Управление курьерами<br>
- Управление товарами
</p>

<h2 align="center">Стек технологий</h2>
<p align="center">
  <img src="https://img.shields.io/badge/-C%23-090909?style=for-the-badge&logo=csharp&logoColor=239120"/>
  <img src="https://img.shields.io/badge/-Windows%20Forms-090909?style=for-the-badge&logo=windows&logoColor=0078D6"/>
  <img src="https://img.shields.io/badge/-SQL-090909?style=for-the-badge&logo=postgresql&logoColor=47c5fb"/>
  <img src="https://img.shields.io/badge/-SSMS-090909?style=for-the-badge&logo=microsoftsqlserver&logoColor=CC2927"/>
  <img src="https://img.shields.io/badge/-Guna%20Framework%202-090909?style=for-the-badge&logo=.net&logoColor=512BD4"/>
</p>

<h2 align="center">Установка</h2>
<p align="center">
1. Клонирование репозитория:<br>
<code>git clone https://github.com/your_username/your_repository.git</code>
</p>

<h2 align="center">Использование</h2>
<p align="center">
1. Запустите приложение<br>
2. Зарегистрируйтесь<br>
3. Авторизируйтесь<br>
4. Введите данные для нового заказа<br>
5. Распечатайте чек
</p>

<h2 align="center">Пример работоспособности программы</h2>

<p align="center">
1. Форма авторизации.<br>
<img src="https://github.com/user-attachments/assets/74eae220-6f7e-4a2d-92d4-380c3c03f7ce" alt="Форма авторизации"/>
</p>

<p align="center">
2. Форма регистрации.<br>
<img src="https://github.com/user-attachments/assets/f3502545-72e2-4115-848c-04db9da994e7" alt="Форма регистрации"/>
</p>

<p align="center">
3. Форма добавления заказа.<br>
<img src="https://github.com/user-attachments/assets/42214ec9-162d-49d0-8a1d-b4fffc3e928c" alt="Форма добавления заказа"/>
</p>

<p align="center">
4. Форма печати чека.<br>
<img src="https://github.com/user-attachments/assets/80bd384d-ed70-4f18-903f-bd71396f7b63" alt="Форма печати чека"/>
</p>

<p align="center">
5. Созданный чек.<br>
<img src="https://github.com/user-attachments/assets/d6489daa-b95d-4299-98fe-0c0cc49a1441" alt="Созданный чек"/>
</p>

<h3 align="center">Структура базы данных</h3>
<p align="center">
Приложение использует СУБД SQL Server Management Studio, в которой находятся следующие таблицы:<br>
- `Order`: заказы<br>
- `Client`: клиенты<br>
- `Tovar`: товары<br>
- `Sotrudniki`: сотрудники<br>
<br>
Также созданы следующие справочные таблицы:<br>
- `Status_Tovara`: статус товара<br>
- `Payment`: метод оплаты<br>
- `Sposob_Vidachi`: способ выдачи<br>
- `Type_Client`: тип клиента<br>
- `Customer_Status`: статус клиента<br>
- `Dolgnost`: должность
</p>

<h3 align="center">Структурная схема базы данных</h3>
<p align="center">
<img src="https://github.com/user-attachments/assets/10c8235b-cb2e-47e6-b1fd-7f78fd719445" alt="Структурная схема базы данных"/>
</p>
