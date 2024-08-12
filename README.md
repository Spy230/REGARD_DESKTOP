# Программа учета и обработки заказа 

### Описание
Это десктопное приложение предназначено для учета и обработки заказов. Оно позволяет управлять заказами, отслеживать их статус, а также генерировать отчеты.

## Функциональные возможности
- Управление заказами
- Отслеживание статуса заказов
- Генерация отчетов
- Интеграция с базами данных
- управление сотрудниками 
- управление клиентами 
- управление курьерами 
- упрапвление товарами 


## стек технологий 

![C#](https://img.shields.io/badge/-C%23-090909?style=for-the-badge&logo=csharp&logoColor=239120)
![Windows Forms](https://img.shields.io/badge/-Windows%20Forms-090909?style=for-the-badge&logo=windows&logoColor=0078D6)
![SQL](https://img.shields.io/badge/-SQL-090909?style=for-the-badge&logo=postgresql&logoColor=47c5fb)
![SQL Server Management Studio](https://img.shields.io/badge/-SSMS-090909?style=for-the-badge&logo=microsoftsqlserver&logoColor=CC2927)
![Guna Framework 2](https://img.shields.io/badge/-Guna%20Framework%202-090909?style=for-the-badge&logo=.net&logoColor=512BD4)


## установка

1. клонирование репозитория 
 git clone https://github.com/your_username/your_repository.git

 ## использование
 1. запустите приложение 
 2. зарегистрируйтесь 
 3. авторизируйтесь 
 4. введите данные для нового заказа
 5. распечатайте чек

 ## пример роботоспособности программы

 1. форма авторизации.

 2. форма регистрации.

 3. форма добавление заказа. 

 4. форма печати чека. 

 5. созданный чек. 

 ### Структура базы данных 

приложение использует СУБД sql management studio , в которой находятся следующие таблицы:
Order - заказы
Client - клиенты 
Tovar - товары  
sotrudniki - сотрудники.
также созданы следующие справочные таблицы:
status_tovara - статус товара .
payment - метод оплаты  
sposob_vidachi - способ выдачи
type_client - тип клиента
Custumer_Status - статус клиента 
dolgnost - должность
 
