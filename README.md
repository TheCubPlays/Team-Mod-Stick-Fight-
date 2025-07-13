## About This Mod

Ever wanted to play **team-modes**, like **2v2,** with your friends in **Stick Fight**? Until now, we had to _act_ like it's a 2v2 by trying not to hit our teammate; But no more! Now, **as long as both you and your friends have this mod installed**, you can experience REAL 2v2 stick fight!

## Features

+ <ins>Teams</ins> - There's currently **2** teams, **your team** & the **enemy team**. By default everyone's in the enemy team until added to yours.

+ <ins>Shared Wins</ins> - Team members **share wins** (Configurable). There's **2 win counters**, one for each team, your win counter's side matches your in-game spawn side. So if you spawn as **Yellow** or **Red**, your win counter will be displayed **on the left**.

+ <ins>Shared Colors</ins> - Team members **share colors** (Configurable). By default, your team's members are **blue** while the enemies are **red** (Configurable).

+ <ins>FriendlyFire Disabled</ins> - **You can't hurt** your team's members. However, **they can hurt you** unless they've **also** added you to their team. (It was made this way to prevent abuse of the mod for cheating purposes).

+ <ins>Configuration</ins> - You can configure all of the mod's features via its **config file** OR even **in-game** by executing certain **commands**.

+ <ins>Commands</ins> - Check below for more info regarding those.

## Commands

All commands are <ins>case insensitive</ins> (So "/prefix" = "/pReFiX")

**Regular Commands**

- `/team list`: Lists the members of your team.
- `/team add <PlayerColor>`: Adds a player to your team using their spawn color (Yellow/Red/Blue/Green).
- `/team remove <PlayerColor>`: Removes a player from your team using their spawn color (Yellow/Red/Blue/Green).
- `/team reset`: Removes everyone from your team.
- `/scouter`: Lists all online players along with their colors. It's useful for `/team` so that you know which player is which color since all team members share the same color and it can be hard to distinguish.

**Configuration Commands**

- `/prefix`: Shows the **prefix** used for **commands** (By default '/').
- `/prefix <Prefix>`: Configure the prefix used for commands **(Must be a character)**
- `/teamWinsToggle`: Shows the current option for **teamWinsToggle** (True/False).
- `/teamWinsToggle <True/False>`: When **true**, team member' wins will be **merged to one** (Visually, not actually).
- `/useTeamColor`: Shows the current option for **useTeamColor** (True/False).
- `/useTeamColor <True/False>`: When **true**, you & your team's members will **share** your team's custom color **(If useColors is also set to true)**. When **false**, you'll have the game's default colors.
- `/useEnemyColor`: Shows the current option for **useEnemyColor** (True/False).
- `/useEnemyColor <True/False>`: When **true**, enemy team members' will **share** their team's custom color **(If useColors is also set to true)**. When **false**, they'll have the game's default colors.
- `/useColors`: Shows the current option for **useColors** (True/False).
- `/useColors <True/False>`: When **true**, all team members` will **share** their team's custom color **(If the corresponding useTeamColor/useEnemyColor options are also true)**. When **false**, they'll have the game's default colors (Regardless of the other 2 options).
- `/teamColor`: Shows the hex color code of your team's color.
- `/teamColor <HexColorCode>`: Configure your team's custom color **(Must use a hex color code with format like #FF0000 or #FF0000FF)**
- `/enemyColor`: Shows the hex color code of the enemy team's color.
- `/enemyColor <HexColorCode>`: Configure the enemy team's custom color **(Must use a hex color code with format like #FF0000 or #FF0000FF)**
- `/reset config`: Resets all configuration settings to default values.
- `/reset prefix`: Resets the prefix used for commands to default value.
- `/reset teamWinsToggle`: Resets the teamWinsToggle option to default value.
- `/reset useTeamColor`: Resets the useTeamColor option to default value.
- `/reset useEnemyColor`: Resets the useEnemyColor option to default value.
- `/reset useColors`: Resets the useColors option to default value.
- `/reset teamColor`: Resets the teamColor option to default value.
- `/reset enemyColor`: Resets the enemyColor option to default value.

### <ins> Notes </ins>

- Your team's win counter won't necessarily have the same colour as your stickman, it has the team's colour. This means that if `useColors` is false (which means custom colors are disabled), your team's win counter will still have the team's colour regardless of you having default colors.
- If the last players standing are of the same team, the round won't end. That's mainly because if that was a thing, the mod could be used as a cheat client.

## How To Use

In order for the mod to work properly, your team's members must have also added you to their team otherwise they'll be able to attack you and you'll be unable to.
So, say you get your 3 friends in your lobby to play. Say you're A and your friends are B, C & D.
Type `/scouter` to see which player corresponds to which color. If you wanna team up with C and their color is red, you'd type `/team add red`. Then, if you're yellow, C will also have to type `/team add yellow`. Now, congratulations, you and C are officially a team. B & D will have to do the same thing with each other. If they do that, you'll now have a functioning 2v2 lobby.
Now, start the fight! :)

<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/ad30d0ef-b1e2-42ce-a4ae-2b555371e736" />

## Installation Guide

1. Download [BepInEx](https://github.com/BepInEx/BepInEx/releases/download/v5.4.19/BepInEx_x86_5.4.19.0.zip).
2. Go to your steam games, right click stick fight and go to `Manage > Browse Local Files`. The `StickFightTheGame` folder should open.
3. Now here, extract the zip file you downloaded. Keep this folder open, we'll need it later.
4. Now start the game and exit once it loads. Now BepInEx should have generated the necessary files & folders.
5. Download the latest version of the team mod (https://github.com/TheCubPlays/Team-Mod-Stick-Fight-/releases/tag/v1.1.1).
6. Now go back to step 3's folder (`StickFightTheGame` folder) and there should be a BepInEx folder now.
7. Go to `BepInEx > Plugins`.
8. In this place, put the **.dll file** from the **zip file**. Either by manually dragging it there or extracting the zip file.
9. You're ready, start the game and enjoy!

## Config File
To access the configuration file, go to your steam games then right click stick fight and click `Managed > Browse Local Files`. Now, click `BepInEx > config`. There should be a file named `cub.plugins.TMOD.cfg`.
There, you can change the mod's configuration settings (Just like you'd do with the commands mentioned above). Any modifications of this file are only applied when you start the game, so you can't modify it while in-game.
Here's an example of a setting to configure:
```
## When 'true' your team's members will share the same custom color (TeamColor). When 'false' your team members will have the default colors of the game. (Yellow/Red/Blue/Green)
# Setting type: Boolean
# Default value: true
useTeamColor = true
```
If you wanted to disable team colors, you'd need to turn `useTeamColor = true` to `useTeamColor = false`. Then save the file and run the game.

Don't change the default values, the `/reset` commands use them to reset your settings.

## Known Issues

- **Grenades** can damage your teammates. However, this isn't necessarily an issue since it'd be weird if grenades didn't damage your teammates assuming they damage you in the vanilla game.
- **Glue gun** can affect your teammates, it won't damage them but it'll still get them stuck.
- **Ice gun** won't damage or slow down your teammates, however it will still give them the ice particle making it look like it'll slow them down even though it won't.
- **Thrusters** will still hurt your teammates with fire.
- Sometimes, the custom colors can have weird bugs. Not sure what causes that yet, the bug itself is very inconsistent. It has a chance to occur when you change the player colors in-game. However I don't think this will happen often. When it happens, you have to rejoin the lobby.
- The command auto-complete doesn't work with parameters yet.

## Credits & License Notice

[Monky](https://github.com/Mn0ky)'s source code was very helpful in the making of this mod, particularly regarding custom colors, chat commands, and overall fundamentals for modding this game.  
Parts of this code are adapted from their amazing [QOL Mod](https://github.com/Mn0ky/QOL-Mod) and are used under the [GNU Lesser General Public License v3.0](https://www.gnu.org/licenses/lgpl-3.0.html).

This project as a whole is licensed under the LGPL v3.0. See the [LICENSE](LICENSE) file for details.

<ins>Note:</ins> Monky's QOL Mod is currently not compatible with this mod.
