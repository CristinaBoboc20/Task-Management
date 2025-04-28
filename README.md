# Task-Management

# Descriere aplicatie
Aceasta aplicatie reprezinta un API folosit pentru a administra task-urile si useri prin operatii de tip CRUD (Creare, Citire, Actualizare si Stergere).
De asemenea, aplicatia cuprinde si functionalitati de auntentificare si autorizare prin Basic Authentication pentru a putea gestiona controlul si permisiunile oferite utilizatorilor.

# Arhitectura aplicatiei
Aplicatia este organizata pe baza modelului de Arhitectura pe Straturi 
- Controllere: stratul de prezentarea => gestioneaza cererile HTTP trimise de client
- Servicii: stratul de aplicatie => controleaza logica aplicatiei
- Repositorii: stratul de acces la date => interactioneaza direct cu baza de date
- Modele: stratul de domeniu => reprezinta structura entitatilor
- DTO-uri: stratul de transfer al datelor => reprezinta obiectele folosite pentru a transfera datele intre client si server
- DbContext: stratul de interactiune cu baza de date => reprezinta contextul de acces la date
- Migrati: gestioneaza modificarile bazei de date
- Autentificare Basic: stratul de autentificare => gestioneaza autentificarea utilizatorilor
- Mapping: stratul de mapare => transforma datele intre modele si DTO-uri
- Middleware pentru gestionarea erorilor: stratul de gestionare a exceptiilor => trateaza erorile aparute in aplicatie
- Testare: stratul de testare => aplicatia include si un proiect pentru teste unitare


## Implementare API

Creatorul este utilizatorul care a creat acel task.
Participantul este utilizatorul caruia i-a fost distribuit taskul.

1. Endpoint-uri pentru Task-uri

   
| Metoda |  Endpoint          | Descriere                                        | Acces                                                  |
| ------ | ------------------ | -------------------------------------------------| -------------------------------------------------------|
| GET    | `api/Task`         | Listeaza task-urile create de utilizatorul curent| User                                                   |
| GET    | `api/Task/{taskId}`| Citeste un task dupa ID                          | Creator sau Participant sau Admin                      |
| POST   | `api/Task/{taskId}`| Creeaza un task nou                              | User                                                   |
| PUT    | `api/Task/{taskId}`| Actualizeaza un task dupa ID                     | Creator sau Participant cu drept de scriere sau Admin  |
| DELETE | `api/Task/{taskId}`| Stergee un task dupa ID                          | Creator sau Admin                                      |

2.Endpointuri pentru Distribuire Task-uri

| Metoda |  Endpoint                              | Descriere                                        | Acces
| ------ | ---------------------------------------| -------------------------------------------------| ------------------
| POST   | `api/TaskSharing/{taskId}/participant` | Distribuie un task catre un utilizator           | Creator sau Admin
| POST   | `api/TaskSharing/{taskId}/participants`| Distribui  un task catre mai multi utilizatori   | Creator sau Admin 

3.Endpointuri pentru Utilizatori

| Metoda |  Endpoint                              | Descriere                                        | Acces |
| ------ | ---------------------------------------| -------------------------------------------------| ----- |
| GET    | `api/Users/`                           | Listeaza toti utilizatorii                       | Admin |
| GET    | `api/Users/{userId}`                   | Citeste un user dupa ID                          | Admin |
| POST   | `api/Users/{userId}`                   | Inregistreaza un nou utilizator                  | Admin |
| DELETE | `api/Users/{deleteId}`                 | Stege un utilizator dupa ID                      | Admin |

4. Endpoint pentru Inregistrare
   
| Metoda |  Endpoint                              | Descriere                                        | Acces |
| ------ | ---------------------------------------| -------------------------------------------------| ----- |
| POST   | `api/Auth/register`                    | Inregistreaza un utilizator nou                  | User  |

# Modele de date
1. User
   
| Proprietate  |  Tip               | Descriere                 |                     
| ------------ | ------------------ | ------------------------- |
| UserId       | Guid               | Id-ul utilizatorului      |                                                          
| UserName     | string             | Numele utilizatorului     |                         
| Password     | string             | Parola hash-uita          |                               
| Role         | enum (User, Admin) | Rolul utilizatorului      |          

2. TaskItem

| Proprietate  |  Tip                                           | Descriere                          |                     
| ------------ | ---------------------------------------------- | -------------------------          |
| TaskId       | Guid                                           | Id-ul task-ului                    |                                                          
| Title        | string                                         | Titlul task-ului                   |                         
| Description  | string                                         | Descrieree                         |                               
| Priority     | enum (Highest, High, Medium, Low, Lowest)      | Prioritatea task-ului              |
| Status       | enum (ToDo, InProgress, InReview, Blocked,Done)| Statusul task-ului                 |
| CreatedData  | DateTime                                       | Data Crearii                       |
| DueDate      | DateTime                                       | Data limita (deadline)            |
| ReporterId   | Guid                                           | Id-ul userului care a creat task-ul|


3. TaskUser => reprezinta task-ul distribuit

| Proprietate  |  Tip                                           | Descriere                                                    |                     
| ------------ | ---------------------------------------------- | -----------------------------------------------------------  |
| TaskUserId   | Guid                                           | Id-ul taskului distribuit                                    |                                                          
| TaskId       | Guid                                           | Id-ul taskului distribuit                                    |      
| UserId       | Guid                                           | Id-ul utilizatorului caruia i-a fost distribuit taskul       |   
| SharedAt     | DateTime                                       | Data cand a fost distribuit taskul                           |                               
| Permisiune   | enum (Read, ReadWrite )                        | Permisiunea care ii este acordata utilizatorului pe acel task|

# Autentificare si Autorizare
## Autentificare
- Se realizeaza prin Basic Authentication, care permite utilizatorului sa trimita username si parola in headerul Authorization.
  Datele sunt codificate in Base64 si sunt trimise sub forma: `Authorization: Basic <Base64(username:password)`
- Parolele sunt hash-uite cu `PasswordHasher<User>` din Microsoft Identity

## Autorizare
- Utilizatorii cu rol de User pot gestiona doar propriile task-uri sau pe cele carora le-au fost distribuite.
- Utilizatorii cu rol de Admin pot gestiona toate task-urile si utilizatorii

