---
name: auth0-setup
description: Configure Auth0 interactivement — pose les questions, écrit appsettings.Development.json et .env.local à partir des fichiers exemples
---

# Auth0 Setup

Configure Auth0 pour ce projet en posant les questions nécessaires puis en écrivant les fichiers de configuration.

## Instructions

Suis ces étapes dans l'ordre. Ne passe pas à l'étape suivante sans avoir la réponse.

### Étape 1 — Lire les fichiers exemples

Lire le contenu de :
- `src/back/appsettings.Development.example.json` → servira de base pour `appsettings.Development.json`
- `src/front/.env.example` → servira de base pour `.env.local`

Ces fichiers définissent la structure exacte et les valeurs par défaut. Les utiliser tels quels — ne pas inventer de structure.

### Étape 2 — Collecter les informations

Poser ces questions à l'utilisateur, **une par une** :

1. **Auth0 Domain** — Dans le dashboard Auth0, Applications → ton application SPA → Settings → *Domain*. Format attendu : `dev-xxx.eu.auth0.com` (sans `https://`).

2. **Client ID** — Sur la même page → *Client ID*.

3. **Audience** — Applications → APIs → ton API custom → *API Audience* (ex. `https://shop-api`). **Attention** : ce n'est pas l'URL de la Management API (`/api/v2/`). Si l'utilisateur n'a pas encore créé d'API custom, lui expliquer qu'il faut aller dans Applications → APIs → Create API, choisir un identifier custom, puis autoriser le SPA sur cette API via l'onglet APIs de son application SPA.

Si l'utilisateur ne sait pas où trouver une valeur, lui donner le chemin exact dans le dashboard avant de continuer.

### Étape 3 — Écrire appsettings.Development.json

Copier le contenu de `src/back/appsettings.Development.example.json` et remplacer les placeholders par les valeurs collectées. Écrire le résultat dans `src/back/appsettings.Development.json`.

### Étape 4 — Écrire .env.local

Copier le contenu de `src/front/.env.example` et remplacer les placeholders par les valeurs collectées. Écrire le résultat dans `src/front/.env.local`.

### Étape 5 — Vérifier le .gitignore

Vérifier que `src/back/appsettings.Development.json` et `src/front/.env.local` sont bien couverts par le `.gitignore`. S'ils ne le sont pas, les ajouter.

### Étape 6 — Confirmer

Informer l'utilisateur que la configuration est terminée et lui rappeler :
- Ne jamais commiter ces fichiers
- Pour tester : lancer le backend (`dotnet watch run` depuis `src/back/`) et le frontend (`npm run dev` depuis `src/front/`), puis vérifier que la redirection Auth0 fonctionne
