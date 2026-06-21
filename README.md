# UE3 Stub Generator

Generates Python type stubs (`.pyi`) from decompressed Unreal Engine 3 script packages for use
with [`pyunrealsdk`](https://github.com/bl-sdk/pyunrealsdk)

# Creating a release

I am not a C# person but google told me this

```powershell
dotnet publish ./UE3StubGenCli/UE3StubGenCli.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishReadyToRun=true -o ./publish/win-x64
dotnet publish ./UE3StubGenCli/UE3StubGenCli.csproj -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -p:PublishReadyToRun=true -o ./publish/linux-x64
```

# Usage

## Requirements

1. An input directory containing decompressed `.u` package files
2. An output directory to write the generated stub files
3. (optional) ruff to format the generated stub files

You can generate the stub files with the following command:

```powershell
UE3StubGenCli.exe --input "path/to/packages" --output "output/path" --import-root "bl1"
```

> It is recommended that you decompress all `*.u` packages into a single directory for the game you are generating stubs for

> Note that the `--import-root` option is optional and defaults to "" and is used to prefix imports i.e., from `bl1.WillowGame` as opposed to `from WillowGame`

# Example Usage

```py
from __future__ import annotations
from typing import TYPE_CHECKING, cast
from unrealsdk import find_object

if TYPE_CHECKING:
    from bl1.WillowGame import ItemPool, InventoryBalanceDefinition

pool: ItemPool = cast("ItemPool", find_object("ItemPool", ""))

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
        "InventoryBalanceDefinition", find_object("InventoryBalanceDefinition", "")
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
