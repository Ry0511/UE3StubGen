# UE3 Stub Generator

Generates Python type stubs (`.pyi`) from decompressed Unreal Engine 3 packages for use
with [`pyunrealsdk`](https://github.com/bl-sdk/pyunrealsdk)

# Usage

```py
from __future__ import annotations
from typing import TYPE_CHECKING, cast
from unrealsdk import find_object

if TYPE_CHECKING:
    from bl1.WillowGame import ItemPool, InventoryBalanceDefinition

pool: ItemPool = cast(ItemPool, find_object("ItemPool", ""))

# @staticmethod
# def SpawnBalancedInventoryFromInventoryBalanceDefinition(
#         InvBalanceDefinition: InventoryBalanceDefinition | None,
#         Quantity: int,
#         GameStage: int,
#         AwesomeLevel: int,
#         ContextSource: Object | None,
#         SpawnedInventory: Out[Array[WillowInventory | None]],
# ) -> tuple[bool, WrappedArray[WillowInventory | None]]:

ret, items = pool.SpawnBalancedInventoryFromInventoryBalanceDefinition(
    InvBalanceDefinition=cast(
        InventoryBalanceDefinition, find_object("InvBalanceDefinition", "")
    ),
    Quantity=0,
    GameStage=50,
    AwesomeLevel=100,
    ContextSource=None,
    SpawnedInventory=[],
)

print(ret, items)
```

# Example Stub File

A generated stub file may look like this:

```py
from unrealsdk.unreal import UClass, WrappedStruct
from bl1.Core import Vector
from bl1.Engine import DamageType, Pawn, TraceHitInfo
from bl1.WillowGame import WillowGib, WillowPawn

class WillowDamageType(DamageType):
    bDirectDamage: bool
    bSeversHead: bool
    bCauseConvulsions: bool
    bUseTearOffMomentum: bool
    bThrowRagdoll: bool
    bLeaveBodyEffect: bool
    bBulletHit: bool
    bVehicleHit: bool
    GibPerterbation: float
    DamageWeaponClass: UClass | None
    DamageWeaponFireMode: int

    @staticmethod
    def SpawnGibEffects(Gib: WillowGib | None) -> None:
        """
        Unreal Path: `WillowGame.WillowDamageType:SpawnGibEffects`

        .. Decompiled UnrealScript:: c
            static function SpawnGibEffects(WillowGib Gib);
        """

    @staticmethod
    def PawnTornOff(DeadPawn: WillowPawn | None) -> None:
        """
        Unreal Path: `WillowGame.WillowDamageType:PawnTornOff`

        .. Decompiled UnrealScript:: c
            static function PawnTornOff(WillowPawn DeadPawn);
        """

    @staticmethod
    def SpawnHitEffect(
        P: Pawn | None,
        Damage: float,
        Momentum: Vector | WrappedStruct,
        HitInfo: TraceHitInfo | WrappedStruct,
        HitLocation: Vector | WrappedStruct,
    ) -> None:
        """
        Unreal Path: `WillowGame.WillowDamageType:SpawnHitEffect`

        .. Decompiled UnrealScript:: c
            static function SpawnHitEffect(Pawn P, float Damage, Vector Momentum, TraceHitInfo HitInfo, Vector HitLocation);
        """

```

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- A directory of **decompressed** `.u` package files for the game you want stubs for
- *(optional)* Python with [`ruff`](https://docs.astral.sh/ruff/) to tidy the generated
  files (recommended - see below)
- *(optional)* [pyright](https://github.com/microsoft/pyright)/Pylance to type-check the
  output
