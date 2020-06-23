from enum import Enum, auto
import random
import console_utilities as cu


def print_list(lst):
    if lst:
        for index, tile_set in enumerate(lst):
            print(f"[{index}]: {tile_set}")
    else:
        print("<none>")


class Color(Enum):
    BLACK = auto()
    BLUE = auto()
    ORANGE = auto()
    RED = auto()

    def __str__(self):
        return self.name.capitalize()


class SetType(Enum):
    RUN = auto()
    GROUP = auto()
    INVALID = auto()

    def __str__(self):
        return self.name.capitalize()


class Player:
    tile_count = 14

    def __init__(self, name):
        self.name = name
        self.tiles = []


class Set:
    def __init__(self):
        self.tiles = []

    def __str__(self):
        return f"{', '.join(map(str, self.tiles))} ({self.set_type()})"

    def set_type(self):
        min_size = 3

        # Check whether set is too short
        if len(self.tiles) < min_size:
            return SetType.INVALID

        first_tile = next((t for t in self.tiles if t.rank != Tile.smiley_rank), None)

        # If all tiles are smileys, this is valid
        # Can consider as either a run or group, as smileys' colours are ignored
        if first_tile is None:
            return SetType.GROUP

        # Check if run
        # Specifically, check if ranks are consecutive and colours are the same (ignoring smileys)
        is_run = all(t.rank == Tile.smiley_rank or (t.rank == first_tile.rank + i and t.color == first_tile.color)
                     for i, t in enumerate(self.tiles))
        if is_run:
            return SetType.RUN

        # Check if group
        # Specifically, check if ranks are same (ignoring smileys)
        is_group = all(t.rank == Tile.smiley_rank or t.rank == first_tile.rank for t in self.tiles)
        if is_group:
            return SetType.GROUP

        return SetType.INVALID  # Set is long enough but is neither a run nor a group

    def valid(self):
        return self.set_type() != SetType.INVALID


class Tile:
    smiley_rank = None
    min_rank = 1  # Inclusive
    max_rank = 14  # Exclusive
    copy_count = 2  # Number of copies of each tile to use

    def __init__(self, color, rank):
        self.color = color
        self.rank = rank

    def __str__(self):
        return f"{self.color} {self.rank}" if self.rank != Tile.smiley_rank else f"{self.color} Smiley"


# TODO: Consider what happens when tiles run out and no valid moves are possible
# TODO: Could consider having option to play with playing cards instead of tiles (suits instead of colours,
#  card ranks instead of numbers, etc.)
# TODO: Could provide messages for invalid sets - e.g. "Set is too short", "Run must contain tiles of the same colour"
# TODO: Check player has actually placed any of their tiles after editing sets (maybe change menu)
# TODO: Maybe add titles to each section of the program
class Game:
    def __init__(self):
        self.players = []
        self.tiles = []
        self.sets = []

    def setup(self):
        self.input_player_names()
        self.generate_tiles()
        self.distribute_tiles()

    def play(self):
        current_player = self.players[0]

        print(f"{current_player.name}'s turn.")

        # Print sets and player's tiles
        print("Sets:")
        print_list(self.sets)
        print("Your tiles:")
        print_list(current_player.tiles)
        print()

        print("Choose an option")
        option = cu.input_option_int([
            "Edit sets",
            "Pass & take tile" if self.tiles else "Pass",
        ])
        print()
        if option == 0:
            self.edit_sets(current_player)
        else:
            if self.tiles:
                current_player.tiles.append(self.tiles.pop())

    def input_player_names(self):
        while True:
            while True:
                default_player_name = f"Player {len(self.players) + 1}"
                player_name = input(
                    f"Enter Player {len(self.players) + 1}'s name (leave blank for \"{default_player_name}\"): ")
                player_name = player_name.strip()

                if player_name == "":
                    player_name = default_player_name

                if player_name in map(lambda p: p.name, self.players):
                    print("Player name already exists. Please enter a different name.")
                else:
                    break

            self.players.append(Player(player_name))
            print(f"Player \"{player_name}\" added.")

            if not cu.input_boolean("Add another player?", default=True):
                break
        print()

    def generate_tiles(self):
        # Generate number tiles
        for rank in range(Tile.min_rank, Tile.max_rank):
            for color in Color:
                for _ in range(Tile.copy_count):
                    self.tiles.append(Tile(color, rank))

        # Generate smiley tiles
        self.tiles.append(Tile(Color.BLACK, Tile.smiley_rank))
        self.tiles.append(Tile(Color.ORANGE, Tile.smiley_rank))

        # Shuffle tiles
        # This way, can just pop() to get a random tile, instead of picking a random item and removing it
        random.shuffle(self.tiles)

    def distribute_tiles(self):
        for player in self.players:
            for _ in range(Player.tile_count):
                player.tiles.append(self.tiles.pop())

    def edit_sets(self, player):
        updated_player_tiles = player.tiles.copy()
        updated_sets = self.sets.copy()

        while True:
            # Choose list to move tile from (source list)
            source_tile_list = self.choose_source_tile_list(updated_sets, updated_player_tiles)

            # Choose tile to remove from source list
            # Remove the tile
            print("Choose which tile to move")
            tile_index = cu.input_option_int(list(map(str, source_tile_list)))
            tile = source_tile_list.pop(tile_index)
            print()

            print("Moving the following tile:")
            print(tile)
            print()

            # Remove empty sets
            updated_sets = [s for s in updated_sets if len(s.tiles) > 0]

            # Choose list to move tile to (destination list)
            destination_tile_list = self.choose_destination_tile_list(updated_sets)

            # Choose index in destination list to insert tile at
            # Insert the tile
            print("Choose position to insert tile at")
            print("The tile will be inserted before the tile at the selected position")
            tile_insert_index = cu.input_option_int(list(map(str, destination_tile_list)) + ["<End>"])
            destination_tile_list.insert(tile_insert_index, tile)
            print()

            # Print updated sets and player's tiles
            print("Updated sets:")
            print_list(updated_sets)
            print("Your updated tiles:")
            print_list(updated_player_tiles)
            print()

            valid = all(map(Set.valid, updated_sets))

            if valid:
                print("Sets are currently VALID")
                print("Your changes will be SAVED if you choose not to continue editing sets")
            else:
                print("Sets are currently INVALID")
                print("Your changes will be LOST if you choose not to continue editing sets")

            option = cu.input_boolean("Continue editing sets?", default=True)
            print()
            if not option:
                break

        # If resulting sets are valid, overwrite player tiles and sets
        if valid:
            player.tiles = updated_player_tiles
            self.sets = updated_sets

    @staticmethod
    def choose_source_tile_list(sets, player_tiles):
        print("Choose where to move a tile from")
        option = cu.input_option_int(list(map(str, sets)) + ["<My tiles>"])
        print()
        if option != len(sets):
            return sets[option].tiles
        return player_tiles

    @staticmethod
    def choose_destination_tile_list(sets):
        print("Choose where to move the tile to")
        option = cu.input_option_int(list(map(str, sets)) + ["<New set>"])
        print()
        if option != len(sets):
            return sets[option].tiles

        new_set = Set()
        sets.append(new_set)
        return new_set.tiles


def main():
    game = Game()
    game.setup()
    game.play()


if __name__ == "__main__":
    main()
