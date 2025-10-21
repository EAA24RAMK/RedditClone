# Reddit Klon, Mini Web API + Blazor

### Lavet af
- **Rasmus Møller Kristensen**  
- **Alfred Juhl Walther**
- **Elias Liebman**
- **Kasper Jørgensen**

---

## Projektindhold

Miniprojekt i faget *Softwarearkitektur*

### **RedditAPI (Server)**
- Minimal Web API
- Bruger **Entity Framework Core** og **SQLite** til database
- Indeholder:
  - `DataService` laver CRUD-logik og SeedData
  - `RedditContext` – læser database
  - API-endpoints til posts og kommentarer

### **RedditAPP (Client)**
- **Blazor frontend**
- Snakker med API via `ApiService`
- Indeholder pages:
  - `PostList` – viser alle posts
  - `PostItem` – viser en post med kommentarer
  - `CreatePost` – opretter nye posts
- Fælles `shared`-projekt bruges til modelklasser

---

## Funktionalitet

En simpel klon af **Reddit**, hvor man kan:

- Se en liste af posts på forsiden
- Klikke på en post for at se dens kommentarer
- Oprette nye posts (tekst eller link)
- Kommentere på eksisterende posts
- Upvote og downvote både posts og kommentarer
- Reddit logo som navigerer tilbage til forsiden

---

## Sådan køres projektet

1. Åbn terminalen i projektets API-mappe  
2. Kør følgende kommandoer:

```bash
dotnet build
dotnet ef database update
dotnet run

1. Kør derefter webappen
