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

1. Форма авторизации.
<p align="center">
  <img src="https://github.com/user-attachments/assets/74eae220-6f7e-4a2d-92d4-380c3c03f7ce" alt="Форма авторизации"/>
</p>

2. Форма регистрации.
<p align="center">
  <img src="https://github.com/user-attachments/assets/f3502545-72e2-4115-848c-04db9da994e7" alt="Форма регистрации"/>
</p>

3. Форма добавления заказа.
<p align="center">
  <img src="https://github.com/user-attachments/assets/42214ec9-162d-49d0-8a1d-b4fffc3e928c" alt="Форма добавления заказа"/>
</p>

4. Форма печати чека.
<p align="center">
  <img src="https://github.com/user-attachments/assets/80bd384d-ed70-4f18-903f-bd71396f7b63" alt="Форма печати чека"/>
</p>

5. Созданный чек.
<p align="center">
  <img src="https://github.com/user-attachments/assets/d6489daa-b95d-4299-98fe-0c0cc49a1441" alt="Созданный чек"/>
</p>

 ### Структура базы данных 

приложение использует СУБД sql management studio , в которой находятся следующие таблицы:
Order - заказы

Client - клиенты 
Tovar - товары  
sotrudniki - сотрудники.
также созданы следующие справочные таблицы:
status_tovara - статус товара
payment - метод оплаты  
sposob_vidachi - способ выдачи
type_client - тип клиента
Custumer_Status - статус клиента 
dolgnost - должность
### Структурная Схема базы данных
<p align="center">
![image](https://github.com/user-attachments/assets/10c8235b-cb2e-47e6-b1fd-7f78fd719445)
</p>
