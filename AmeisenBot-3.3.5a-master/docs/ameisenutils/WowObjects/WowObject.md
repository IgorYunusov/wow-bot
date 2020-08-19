# WoW Objects

## Structure

<pre>
WowObject
┣━ Unit
┃  ┗━ Player
┃     ┗━ Me
┣━ GameObject
┣━ DynObject
┣━ Corpse
┣━ Container
┗━ Item
</pre>

* * *

## WowObject

This is the base class for all the things we will read from WoW's object manager. It will only conatin the base data that every Object will have.

| Property    | Type  | Description                                              | BaseAddress Offset |
|-------------|-------|----------------------------------------------------------|--------------------|
| BaseAddress | uint  | Memory address of the object                             | 0x0                |
| Descriptor  | uint  | Will be used to find things like Health, Mana etc. later | 0x8                |
| Guid        | ulong | GUID of the object                                       | 0x30               |

* * *

## Unit

This is the base class for all Units including the player and ourself.

| Property      | Type        | Description                 | BaseAddress Offset    | Descriptor Offset |
|---------------|-------------|-----------------------------|-----------------------|-------------------|
| Name          | string      | Name of the Unit (24 chars) | ((0x964) + 0x05C)     | -                 |
| Pos           | Vector3     | X, Y, Z coordinated (float) | 0x798 (+ 0x8, + 0x16) | -                 |
| Rotation      | float       | Rotation of the Unit        | 0x7A8                 | -                 |
| TargetGUID    | ulong       | The targets GUID            | -                     | 0x48              |
| Level         | int         | Level of the Unit           | -                     | 0xD8              |
| Health        | int         | Health of the Unit          | -                     | 0x60              |
| MaxHealth     | int         | MaxHealth of the Unit       | -                     | 0x80              |
| Mana          | int         | Mana of the Unit            | -                     | 0x64              |
| MaxMana       | int         | MaxMana of the Unit         | -                     | 0x84              |
| Rage          | int         | Rage of the Unit            | -                     | 0x68              |
| MaxRage       | int         | MaxRage of the Unit         | -                     | -                 |
| Energy        | int         | Energy of the Unit          | 0xFC0                 | -                 |
| MaxEnergy     | int         | MaxEnergy of the Unit       | -                     | -                 |
| RuneEnergy    | int         | RuneEnergy of the Unit      | 0x19D4                |                   |
| MaxRuneEnergy | int         | MaxRuneEnergy of the Unit   | -                     | -                 |
| UFlags        | BitVector32 | UserFlags                   | -                     | 0xEC              |
| UFlags2       | BitVector32 | UserFlags extended          | -                     | 0xF0              |
| DynamicFlags  | BitVector32 | DynamicFlags                | -                     | 0x240             |

* * *

## Player

This is the base class for all Players, the only difference between this class and the Unit class is the reading of the Name.

* * *

## Me

This is the base class for ourself.

| Property         | Type           | Description                 | PlayerBase Offset | Static Offset                                                           |
|------------------|----------------|-----------------------------|-------------------|-------------------------------------------------------------------------|
| Race             | WowRace (int)  | Your Race                   | -                 | 0xC79E88                                                                |
| Class            | WowClass (int) | Your Class                  | -                 | 0xC79E89                                                                |
| Name             | String         | Your Name                   | -                 | 0xC79D18                                                                |
| Exp              | int            | Expirience                  | 0x3794            | -                                                                       |
| MaxExp           | int            | Your needed Exp for Levelup | 0x3798            | -                                                                       |
| PartyLeaderGuid  | ulong          | The party/raidleaders GUID  | -                 | 0xBD1968 or 0xBD1990                                                    |
| PartymemberGuids | List<ulong>    | List of party/raidmembers   | -                 | 0xBD1948 + (0x8 * player (1 - 4)) or 0xBF8258 + (0x50 * player (1 - 40) |
| PetGuid          | ulong          | The GUID of your pet        | -                 | 0xC234D0                                                                |
| PlayerBase       | int            | Your PlayerBaseAdress       | -                 | ((0xCD87A8) + 0x34) + 0x24)                                             |
## GameObject

Currently unused.

## DynObject

Currently unused.

## Corpse

Currently unused.

## Container

Currently unused.

## Item

Currently unused.