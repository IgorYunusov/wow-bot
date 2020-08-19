# AmeisenBot

* * *

## A bot for WoW 3.3.5a 12340

<aside class="warning">
    At this time this bot is only a playground for me
    to learn and test stuff, but it will get more and
    more stable in the near future.
</aside>

![alt text](https://github.com/Jnnshschl/WoW-3.3.5a-Bot/blob/master/images/mainscreen.PNG?raw=true "Mainscreen")

This bot is going to have a strong focus on party/raid assistance. My goal is to complete a raid with me and 9 bots. Further the bot should do some things on its own, for example repairing its equipment or farm stuff that is needed for the raid. Maybe if this features work flawlessly i'll add more stuff to it. There is a high chance that this bot will do battlegrounds on its own in the future, because that's a thing in wow that i used to play back in the days for a very long time.

* Base
    * Auto Revive/Run to the corpse
* Additional    
    * Change/Manage realmlists
    * Auto login
    * Auto launch WoW
    * Auto limit WoW's FPS
    * Auto set ulow GFX settings
* Party/Raid
    * Combat
        * DPS
        * Healing
        * *Tank (not tested)*
        * See below for currently supported specs/classes
    * Following
    * Auto accept party invite
* Navmesh based movement
    * See [AmeisenNavigation](https://github.com/Jnnshschl/AmeisenNavigation)
* Inventory/Equipment
    * Inventory/Equipment management
    * Auto equip better items
    * *Auto roll on items (W.I.P)*
* AI stuff
    * Remember units and what the bot can do at this unit
    * Auto repair equipment
        * Use the mammoth to repair if the bot got it
    * Auto sell grays from bags
    * Do random emotes when at idle
* LUA
    * Execute LUA
    * Capture WoW's events

Supported Classes:

**Class** | **Spec A** | **Spec B** | **Spec C**
------------ | ------------- | ------------- | -------------
**Druid** | ❌ Balance | ❌ Feral | ✔️ Restoration 
**Hunter** | ✔️ Marksmanship | ❌ Beast Mastery | ❌ Survival
**Mage** | ✔️ Fire | ❌ Frost | ❌ Arcane
**Paladin** | ❌ Holy | ✔️ Retribution | ❌ Protection
**Priest** | ✔️ Holy | ❌ Discipline | ✔️ Shadow
**Rogue** | ✔️ Combat | ❌ Assasination | ❌ Sublety
**Shaman** | ❌ Elemental | ❌Enhancement | ❌ Restoration
**Warlock** | ✔️ Affliction | ❌ Demonology | ❌ Destruction
**Warrior** | ❌ Arms | ✔️ Fury | ❌ Protection
**Death Knight** | ❌ Blood | ❌ Frost | ❌ Unholy