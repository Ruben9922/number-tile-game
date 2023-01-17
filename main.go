package main

import (
	"fmt"
	"log"
	"math/rand"
	"sort"
	"strconv"
	"strings"

	"golang.org/x/exp/slices"
)

type player struct {
	name  string
	tiles []tile
}

type tile struct {
	rank  int
	color color
}

func (tile tile) String() string {
	return fmt.Sprintf("%s %s", tile.color.String(), rankToString(tile.rank))
}

type color int

const (
	Black color = iota
	Blue
	Orange
	Red
	colorCount int = 4 // Not sure if better way to do this
)

//go:generate go run golang.org/x/tools/cmd/stringer -type=color

const playerTileCount int = 14

const smileyRank int = 0

func main() {
	players, tiles, sets := setup()
	play(players, tiles, sets)
}

func play(players []player, tiles []tile, sets []set) {
	currentPlayerIndex := 0
	consecutivePasses := 0

	anyPlayerHasNoTiles := !slices.ContainsFunc(players, func(p player) bool {
		return len(p.tiles) == 0
	})

	for consecutivePasses <= len(players) && anyPlayerHasNoTiles {
		currentPlayer := &players[currentPlayerIndex]
		fmt.Printf("%s's turn", currentPlayer.name)
		fmt.Println()

		const (
			EditSets = iota
			EndTurn
		)

		valid := false
		var o option
		for o.value != EndTurn {
			fmt.Println("Sets:")
			fmt.Println(sliceToString(sets))
			fmt.Println("Your tiles:")
			fmt.Println(sliceToString(currentPlayer.tiles))

			var endTurnText string
			if valid {
				endTurnText = "End turn"
			} else if len(tiles) == 0 {
				endTurnText = "Pass"
			} else {
				endTurnText = "Pass and take tile"
			}

			o = inputOption("Choose an option", []option{
				{name: "Edit sets", value: EditSets},
				{name: endTurnText, value: EndTurn},
			})
			switch o.value {
			case EditSets:
				valid = editSets(currentPlayer, &sets)
			case EndTurn:
				if len(tiles) > 0 && !valid {
					newTile := tiles[0]
					tiles = tiles[1:]
					currentPlayer.tiles = append(currentPlayer.tiles, newTile)
					fmt.Printf("Picked up: %s\n", newTile)
					sortTiles(currentPlayer.tiles)
				}
			}

			if o.value == EndTurn && len(tiles) == 0 {
				consecutivePasses++
			} else {
				consecutivePasses = 0
			}
		}

		currentPlayerIndex++
		currentPlayerIndex %= len(players)
	}

	winningPlayer := computeWinningPlayer(players)
	if winningPlayer.tiles != nil {
		fmt.Printf("%s wins!", winningPlayer.name)
	}
}

func computeWinningPlayer(players []player) (winningPlayer player) {
	for _, p := range players {
		if winningPlayer.tiles == nil || len(p.tiles) < len(winningPlayer.tiles) {
			winningPlayer = p
		}
	}
	return
}

func copySets(sets []set) []set {
	setsCopy := make([]set, 0, len(sets))
	for _, s := range sets {
		sCopy := make([]tile, len(s))
		copy(sCopy, s)
		setsCopy = append(setsCopy, sCopy)
	}
	return setsCopy
}

func editSets(player *player, sets *[]set) bool {
	updatedPlayerTiles := make([]tile, len(player.tiles))
	copy(updatedPlayerTiles, player.tiles)

	updatedSets := copySets(*sets)

	finished := false
	var valid bool
	for !finished {
		sourceTileList := chooseSourceTileList(&updatedPlayerTiles, &updatedSets)
		tileRemoveIndex := chooseTileRemoveIndex(*sourceTileList)
		tile := (*sourceTileList)[tileRemoveIndex]
		*sourceTileList = slices.Delete(*sourceTileList, tileRemoveIndex, tileRemoveIndex+1)

		fmt.Println("Moving the following tile:")
		fmt.Println(tile)

		updatedSets = removeEmptySets(updatedSets)

		destinationTileList := chooseDestinationTileList(&updatedSets)
		tileInsertIndex := chooseTileInsertIndex(*destinationTileList)
		*destinationTileList = slices.Insert(*destinationTileList, tileInsertIndex, tile)

		fmt.Println("Updated sets:")
		fmt.Println(sliceToString(updatedSets))
		fmt.Println("Your updated tiles:")
		fmt.Println(sliceToString(updatedPlayerTiles))

		setsValid := !slices.ContainsFunc(updatedSets, func(s set) bool {
			return !s.IsValid()
		})
		playerTilesPlaced := len(updatedPlayerTiles) < len(player.tiles)
		valid = setsValid && playerTilesPlaced

		if valid {
			fmt.Println("Sets are currently VALID")
			fmt.Println("Your changes will be SAVED if you choose not to continue editing sets")
		} else {
			if setsValid {
				fmt.Println("Sets are currently VALID but you didn't place any of your tiles")
			} else {
				fmt.Println("Sets are currently INVALID")
			}
			fmt.Println("Your changes will be LOST if you choose not to continue editing sets")
		}

		finished = !inputBoolean("Continue editing sets?")
	}

	if valid {
		player.tiles = updatedPlayerTiles
		*sets = updatedSets
	}

	return valid
}

func removeEmptySets(sets []set) (updatedSets []set) {
	for _, s := range sets {
		if len(s) != 0 {
			updatedSets = append(updatedSets, s)
		}
	}
	return
}

//func isValid(playerTiles []tile, updatedPlayerTiles []tile, updatedSets []set) bool {
//}

func chooseSourceTileList(playerTiles *[]tile, sets *[]set) *[]tile {
	options := make([]option, 0, len(*sets)+1)
	options = append(options, option{
		name:  "<My tiles>",
		value: 0,
	})
	for i, s := range *sets {
		options = append(options, option{
			name:  s.String(),
			value: i + 1,
		})
	}
	option := inputOption("Choose where to move a tile from", options)

	// todo: change to use list as value (?)
	if option.value == 0 {
		return playerTiles
	} else {
		return (*[]tile)(&(*sets)[option.value-1])
	}
}

func chooseDestinationTileList(sets *[]set) *[]tile {
	options := make([]option, 0, len(*sets)+1)
	for i, s := range *sets {
		options = append(options, option{
			name:  s.String(),
			value: i,
		})
	}
	options = append(options, option{
		name:  "<New set>",
		value: len(*sets),
	})
	option := inputOption("Choose where to move the tile to", options)

	// todo: change to use list as value (?)
	if option.value == len(*sets) {
		newSet := make(set, 0)
		*sets = append(*sets, newSet)
		return (*[]tile)(&(*sets)[len(*sets)-1])
	}

	return (*[]tile)(&(*sets)[option.value])
}

func chooseTileRemoveIndex(tiles []tile) int {
	options := make([]option, 0, len(tiles))
	for i, t := range tiles {
		options = append(options, option{
			name:  t.String(),
			value: i,
		})
	}

	tileRemoveIndex := inputOption("Choose which tile to move", options).value
	return tileRemoveIndex
}

func chooseTileInsertIndex(tiles []tile) int {
	options := make([]option, 0, len(tiles)+1)
	for i, t := range tiles {
		options = append(options, option{
			name:  t.String(),
			value: i,
		})
	}
	options = append(options, option{
		name:  "<End>",
		value: len(tiles),
	})

	tileInsertIndex := inputOption("Choose position to insert the tile at", options).value
	return tileInsertIndex
}

type option struct {
	name  string
	value int
}

func inputOption(prompt string, options []option) option {
	fmt.Println(prompt)

	optionNames := make([]string, 0, len(options))
	for _, o := range options {
		optionNames = append(optionNames, o.name)
	}
	fmt.Println(sliceToString(optionNames))

	valid := false
	var optionIndex int
	for !valid {
		fmt.Printf("Select an option [0..%d]: ", len(options)-1)
		var inputString string
		_, err := fmt.Scanln(&inputString)
		if err != nil && err.Error() != "unexpected newline" {
			log.Fatal(err)
		}

		optionIndex, err = strconv.Atoi(inputString)
		valid = err == nil && optionIndex >= 0 && optionIndex < len(options)
		if !valid {
			fmt.Printf("Input must be a valid integer between %d and %d inclusive\n", 0, len(options)-1)
		}
	}

	return options[optionIndex]
}

func setup() ([]player, []tile, []set) {
	playerNames := inputPlayerNames()
	players := createPlayers(playerNames)
	tiles := generateTiles()
	distributeTiles(players, &tiles)

	sets := make([]set, 0)

	return players, tiles, sets
}

func distributeTiles(players []player, tiles *[]tile) {
	for i := range players {
		p := &players[i]
		p.tiles = append(p.tiles, (*tiles)[:playerTileCount]...)
		*tiles = (*tiles)[playerTileCount:]

		sortTiles(p.tiles)
	}
}

func sortTiles(tiles []tile) {
	sort.Slice(tiles, func(i, j int) bool {
		if tiles[i].rank == tiles[j].rank {
			return tiles[i].color < tiles[j].color
		}
		return tiles[i].rank < tiles[j].rank
	})
}

func rankToString(rank int) string {
	if rank == 0 {
		return "Smiley"
	}
	return strconv.Itoa(rank)
}

func sliceToString[T any](list []T) string {
	if len(list) == 0 {
		return "<None>"
	}

	listStrings := make([]string, 0, len(list))
	for index, item := range list {
		listStrings = append(listStrings, fmt.Sprintf("[%d]: %s", index, item))
	}

	return strings.Join(listStrings, "\n")
}

func inputBoolean(prompt string) bool {
	valid := false
	const trueValue string = "y"
	const falseValue string = "n"
	var value bool
	for !valid {
		fmt.Printf("%s [y/n]: ", prompt)
		var inputString string
		_, err := fmt.Scanln(&inputString)
		if err != nil && err.Error() != "unexpected newline" {
			log.Fatal(err)
		}

		if strings.EqualFold(inputString, trueValue) {
			value = true
			valid = true
		} else if strings.EqualFold(inputString, falseValue) {
			value = false
			valid = true
		} else {
			valid = false
			fmt.Println("Input must be \"y\" or \"n\" (case insensitive)")
		}
	}
	return value
}

func inputPlayerNames() []string {
	playerNames := make([]string, 0)
	finished := false
	for !finished {
		valid := false
		for !valid {
			var playerName string
			defaultPlayerName := fmt.Sprintf("Player %d", len(playerNames)+1)
			fmt.Printf("Player %d name [leave blank for \"%s\"]: ", len(playerNames)+1, defaultPlayerName)
			_, err := fmt.Scanln(&playerName)
			if err != nil && err.Error() != "unexpected newline" {
				log.Fatal(err)
			}

			if playerName == "" {
				playerName = defaultPlayerName
			}

			valid = !slices.Contains(playerNames, playerName)
			if valid {
				playerNames = append(playerNames, playerName)
				fmt.Printf("Player \"%s\" added\n", playerName)
			} else {
				fmt.Println("Player name already exists; please choose a different name")
			}
		}
		finished = !inputBoolean("Input another player?")
	}
	return playerNames
}

func createPlayers(playerNames []string) []player {
	players := make([]player, 0, len(playerNames))
	for _, playerName := range playerNames {
		p := player{name: playerName, tiles: make([]tile, 0, playerTileCount)}
		players = append(players, p)
	}
	return players
}

func generateTiles() []tile {
	const tileMinRank = 1
	const tileMaxRank = 14
	const tileCopyCount = 2

	numberTiles := make([]tile, 0, (tileMaxRank-tileMinRank)*tileCopyCount*colorCount)
	for color1 := 0; color1 < colorCount; color1++ {
		for rank := tileMinRank; rank < tileMaxRank; rank++ {
			for i := 0; i < tileCopyCount; i++ {
				tile := tile{
					rank:  rank,
					color: color(color1),
				}
				numberTiles = append(numberTiles, tile)
			}
		}
	}

	smileyTiles := []tile{
		{
			rank:  smileyRank,
			color: Orange,
		},
		{
			rank:  smileyRank,
			color: Orange,
		},
	}

	tiles := append(numberTiles, smileyTiles...)

	// Shuffle tiles
	// This way, can just get an element from the start or end, instead of having to pick a random element
	rand.Shuffle(len(tiles), func(i, j int) {
		tiles[i], tiles[j] = tiles[j], tiles[i]
	})

	return tiles
}
