# Mod Introduction

This Mod adds the function of crafting wonders for Pathfinder: Wrath of the Righteous. The Mod is currently in development and may have frequent bugs. If you encounter any problems, please feel free to submit an Issue to give feedback.

# Mod Installation

1. Download and install UnityModManager (UMM for short)
2. Download this Mod
3. Use UnityModManager or ModFinder to install this Mod
4. Start the game, load the save, and press Ctrl+F10 to open the UMM interface in the game
5. Find the CraftMaster Mod interface and use the Mod function

# Risk Warning

Some of the functions used by this Mod add new data to the game save

Removing this Mod may cause corrupted save files

And this Mod is currently in development and may not be very stable, please backup your save files in time.

# Crafting Rules

In this Mod, there are two key indicators when crafting ordinary items or magic items: crafting points and crafting DC

Crafting points are the points required to craft the item. When the points are reached, the item is crafted.

Crafting DC is the check that needs to be made when crafting the item. In the game, a crafting check (DC) is made every hour, and the crafting point progress is increased according to the check result.

For every 5 points that the DC check result exceeds the crafting DC, the crafting point progress is additionally increased by 1 point

## Ordinary Equipment

Crafting points are 10

Crafting DC is 10

## Equipment Enchantment

Crafting points are 10 + new enchantment points ^ 2 * 10

Crafting DC is 20 + new enchantment points * 5

## Wand Crafting

Crafting points are 10 + caster level * 5 + spell level * 10

Crafting DC is 20 + caster level * 2 + spell level * 2

## Related Feats

### General Feats

| Feat Name                     | Feat Prerequisite              | Feat Effect                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        |
| ----------------------------- | ------------------------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Craft Magic Weapons and Armor | Caster level reaches 5th level | You can craft magic armor, shields and weapons.<br />To enchant a weapon, a set of armor or a shield, you need to make a crafting check (DC) every hour. After passing the check, you gain 1 point of crafting progress. For every 5 points that the check result exceeds the difficulty, you gain an additional 1 point of crafting progress.<br />The number of checks required to enchant is 10 + new enchantment points ^ 2 * 10<br />The difficulty of enchanting is 20 + new enchantment points * 5        |
| Craft Wand                    | Caster level reaches 5th level | You can craft a wand of any spell you know of 4th level or lower.<br />To craft a wand, you need to make a crafting check (DC) every hour. After passing the check, you gain 1 point of crafting progress. For every 5 points that the check result exceeds the difficulty, you gain an additional 1 point of crafting progress.<br />The number of checks required to craft a wand is 10 + caster level * 5 + spell level * 10<br />The difficulty of crafting a wand is 20 + caster level * 2 + spell level * 2 |

### Mythic Feats

| Feat Name           | Feat Prerequisite | Feat Effect                                                                                                                        |
| ------------------- | ----------------- | ---------------------------------------------------------------------------------------------------------------------------------- |
| Craft Wand (Mythic) | Craft Wand        | Your wand crafting skills are so excellent that you can craft wands above 4th level and add your mythic level to the DC for wands. |

### Mythic Abilities

| Feat Name               | Feat Prerequisite | Feat Effect                                                                                                                                                                                                   |
| ----------------------- | ----------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Masterful Craftsmanship | None              | Your ability to craft magic items has reached perfection. When crafting magic items, successfully passing the DC check will give you an additional half of your mythic level (rounded up) of crafting points. |
