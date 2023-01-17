package main

import (
	"fmt"
	"strings"
)

type set []tile

type setType int

const (
	Run setType = iota
	Group
	Invalid
)

//go:generate go run golang.org/x/tools/cmd/stringer -type=setType

func (s set) ComputeSetType() (setType, string) {
	const minLength int = 3

	// Check whether set is too short
	if len(s) < minLength {
		return Invalid, fmt.Sprintf("Set is too short (minimum set length is %d)", minLength)
	}

	// Obtain first tile that isn't a smiley
	refTileIndex := -1
	for i, t := range s {
		if t.rank != smileyRank {
			refTileIndex = i
			break
		}
	}

	// If there is no tile that isn't a smiley, there are 3+ tiles that are all smileys, which is valid
	// Can consider this as either a run or group, as smileys' colours are ignored
	if refTileIndex == -1 {
		return Run, ""
	}

	refTile := s[refTileIndex]

	var isRun bool
	for i, t := range s {
		isRun = t.rank == smileyRank || (t.rank == refTile.rank+i && t.color == refTile.color)

		if !isRun {
			break
		}
	}

	if isRun {
		return Run, ""
	}

	var isGroup bool
	for _, t := range s {
		isGroup = t.rank == smileyRank || t.rank == refTile.rank

		if !isGroup {
			break
		}
	}

	if isGroup {
		return Group, ""
	}

	var looksLikeRun bool
	for i, t := range s {
		looksLikeRun = t.rank == smileyRank || t.rank == refTile.rank+i

		if !looksLikeRun {
			break
		}
	}

	if looksLikeRun {
		return Invalid, "Tiles in a run must be of the same colour"
	}

	return Invalid, ""
}

func (s set) IsValid() bool {
	setType, _ := s.ComputeSetType()
	return setType != Invalid
}

func (s set) String() string {
	tileStrings := make([]string, 0, len(s))
	for _, t := range s {
		tileStrings = append(tileStrings, t.String())
	}

	setType, errorMessage := s.ComputeSetType()

	var setTypeString string
	if errorMessage != "" {
		setTypeString = fmt.Sprintf("%s - %s", setType, errorMessage)
	} else {
		setTypeString = setType.String()
	}

	return fmt.Sprintf("%s (%s)", strings.Join(tileStrings, ", "), setTypeString)
}
