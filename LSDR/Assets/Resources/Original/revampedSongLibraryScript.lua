function getSong(style, songNumber)
    local songIdx = Random.IntMax(#songList.Songs)
    return songList.Songs[songIdx]
end