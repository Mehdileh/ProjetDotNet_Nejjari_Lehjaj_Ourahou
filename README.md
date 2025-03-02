# Projet Gauniv - Plateforme de Gestion de Jeux Vidéo

## Présentation du projet
Gauniv est une plateforme de distribution de contenu et un éditeur de jeux vidéo développé en **ASP.Net Core** et **WPF/MAUI**. Elle permet aux utilisateurs de :
- **Explorer** une bibliothèque de jeux vidéo.
- **Télécharger et gérer** leurs jeux.
- **Jouer à des jeux** avec des fonctionnalités en ligne.
- **Gérer leurs profils utilisateurs** et paramètres de l'application.

Ce projet est conçu en utilisant une architecture modulaire, incluant un **serveur web**, un **client Windows**, un **serveur de jeu** et un **jeu** développé avec **Godot, Unity ou WPF**.


## Architecture du projet
La solution contient plusieurs modules :
- **Gauniv.WebServer** : Serveur web en **ASP.Net Core** (API REST pour la gestion des jeux et utilisateurs).
- **Gauniv.Client** : Client Windows en **WPF/MAUI** pour naviguer et gérer les jeux.
- **Gauniv.Network** : Module de communication entre le client et le serveur.
- **Gauniv.GameServer** : Serveur de jeu orchestrant les parties multijoueurs.
- **Gauniv.Game** : Jeu développé en **C# (Godot, Unity, WPF, WinForms, MAUI, etc.)**.

Le projet repose sur **Docker** pour le déploiement des services (base de données, cache Redis, serveur web).

---

## Installation et Exécution
### Prérequis
- **.NET 8+**
- **Docker et Docker Compose**
- **Visual Studio 2022+ avec le support .NET MAUI/WPF**

---

### Étapes d'installation
1. **Cloner le projet** :
   ```bash
   git clone https://github.com/Mehdileh/ProjetDotNet_Nejjari_Lehjaj_Ourahou.git
   ```
2. **Démarrer les services backend avec Docker** :
   ```bash
   docker-compose up -d
   ```
3. **Lancer le serveur web** :
   ```bash
   cd Gauniv.WebServer
   dotnet run
   ```
4. **Lancer le client Windows** :
   ```bash
   cd Gauniv.Client
   dotnet run
   ```
---

## 🔑 Identifiants de Connexion
Voici les identifiants pour tester l'application avec les rôles Admin et Player :

| Rôle   | Email               | Mot de passe |
|-----------|---------------------|--------------|
| **ADMIN** | `admin@example.com`    | `admin123`   |
| **PLAYER**  | `player@example.com` | `player123`    |

---

## API REST
L'API REST de `Gauniv.WebServer` est documentée avec **Swagger** et accessible à l'adresse suivante :
```
http://localhost:5231/swagger/index.html
```

### Authentification
- `POST /api/auth/register` → Inscription d'un utilisateur.
- `POST /api/auth/login` → Connexion d'un utilisateur.
- `POST /api/auth/assign-role` → Attribution d'un rôle.
- `GET /api/auth/check-admin` → Vérification du rôle administrateur.

### Gestion des catégories
- `GET /api/categories` → Liste des catégories.
- `POST /api/categories` → Création d'une catégorie.
- `PUT /api/categories/{id}` → Mise à jour d'une catégorie.
- `DELETE /api/categories/{id}` → Suppression d'une catégorie.

### Gestion des jeux
- `GET /api/games` → Liste des jeux.
- `POST /api/games` → Ajout d'un jeu.
- `GET /api/games/{id}` → Détails d'un jeu.
- `DELETE /api/games/{id}` → Suppression d'un jeu.
- `GET /api/games/{id}/download` → Téléchargement d'un jeu.
- `POST /api/games/{id}/categories/{categoryId}` → Ajout d'une catégorie à un jeu.
- `DELETE /api/games/{id}/categories/{categoryId}` → Suppression d'une catégorie d'un jeu.
- `POST /api/games/{id}/buy` → Achat d'un jeu.
- `GET /api/games/owned` → Liste des jeux possédés.
- `GET /api/games/owned/{id}` → Détails d'un jeu possédé.
- `DELETE /api/games/{id}/uninstall` → Désinstallation d'un jeu.

---

## Fonctionnalités principales
- **Gestion des jeux** : affichage, filtres, téléchargement, suppression.
- **Affichage des détails des jeux** : description, statuts, catégories.
- **Gestion du profil utilisateur** : préférences, identifiants, stockage des jeux.
- **Lancement et gestion des jeux** : statut (téléchargé, prêt, en jeu), démarrage/arrêt.
- **Multijoueur** : communication avec le serveur de jeu.

---

## Auteurs
- **NEJJARI Mohamed Nizar**
- **LEHJAJ Mehdi**
- **OURAHOU Yassir**


