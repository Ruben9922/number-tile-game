from enum import Enum, auto
import random
import console_utilities as cu


class Color(Enum):
    BLACK = auto()
    BLUE = auto()
    ORANGE = auto()
    RED = auto()


class SetType(Enum):
    RUN = auto()
    GROUP = auto()


class Player:
    tile_count = 14

    def __init__(self, name):
        self.name = name
        self.tiles = []


class Set:
    def __init__(self):
        self.tiles = []

    def set_type(self):
        min_size = 3

        # Check whether set is too short
        if len(self.tiles) < min_size:
            return None

        first_tile = next((t for t in self.tiles if t.rank == Tile.smiley_rank), None)

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

        return None  # Set is long enough but is neither a run nor a group


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

        # Print the tiles on the table
        print("Sets:")
        if self.sets:
            for index, tile_set in enumerate(self.sets):
                print(f"[{index}]: {tile_set}")
        else:
            print("<none>")

        # Print the player's tiles
        print("Your tiles:")
        if current_player.tiles:
            for index, tile in enumerate(current_player.tiles):
                print(f"[{index}]: {tile}")
        else:
            print("<none>")

        option = cu.input_option_int([
            "Edit sets",
            "Pass & take tile" if self.tiles else "Pass",
        ])
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

            # Choose list to move tile to (destination list)
            destination_tile_list = self.choose_destination_tile_list(updated_sets)

            # Choose index in destination list to insert tile at
            # Insert the tile
            print("Choose position to insert tile at")
            tile_insert_index = cu.input_option_int(list(map(str, destination_tile_list)))
            destination_tile_list.insert(tile_insert_index, tile)

            valid = all(map(Set.valid, updated_sets))

            if valid:
                print("Sets are currently VALID")
                print("Your changes will be SAVED if you choose not to continue editing sets")
            else:
                print("Sets are currently INVALID")
                print("Your changes will be LOST if you choose not to continue editing sets")

            if not cu.input_boolean("Continue editing sets?", default=True):
                break

        # If resulting sets are valid, overwrite player tiles and sets
        if valid:
            player.tiles = updated_player_tiles
            self.sets = updated_sets

    def choose_source_tile_list(self, sets, player_tiles):
        if sets:
            print("Choose where to move a tile from")
            option = cu.input_option_int(list(map(str, self.sets)) + ["<My tiles>"])
            if option != len(self.sets):
                return sets[option]

        return player_tiles

    def choose_destination_tile_list(self, sets):
        if sets:
            print("Choose where to move the tile to")
            option = cu.input_option_int(list(map(str, self.sets)) + ["<New set>"])
            if option != len(self.sets):
                return sets[option]

        new_set = Set()
        self.sets.append(new_set)
        return new_set.tiles


def main():
    game = Game()
    game.setup()
    game.play()


if __name__ == "__main__":
    main()
