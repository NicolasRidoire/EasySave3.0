# EASY SAVE

EasySave - Version 1.0

Bienvenue dans EasySave, une application de sauvegarde simple en ligne de commande utilisant .Net Core.

## Installation
Clonez ce référentiel :

`
Copy code
git clone [lien du référentiel]
cd EasySave
Assurez-vous d'avoir .Net Core installé. Sinon, téléchargez-le depuis le site officiel.`

Installez les dépendances :


Copy code
dotnet restore
Configuration
Éditez le fichier de configuration appsettings.json pour définir les répertoires et les paramètres nécessaires.

Assurez-vous que les emplacements des fichiers de journal (log.json) et d'état (state.json) sont adaptés aux serveurs des clients.

## Utilisation

Exécution de sauvegardes :

`
Copy code
dotnet run -- backup 1-3`
Exécute automatiquement les sauvegardes 1 à 3.
Exécution séquentielle :

`
Copy code
dotnet run -- backup 1 ;3`

Exécute automatiquement les sauvegardes 1 et 3.

## Fonctionnalités

Création de jusqu'à 5 travaux de sauvegarde.
Sauvegarde de fichiers depuis des disques locaux, externes et des lecteurs réseau.
Journal d'historique des sauvegardes (log.json).
Fichier d'état en temps réel (state.json).
Interface en anglais et en français.
Formats de Fichier
Les fichiers de journal et d'état sont au format JSON pour une lecture facile.
Assurez-vous d'ajouter des retours à la ligne pour une lisibilité optimale dans Notepad.
