function getSong(style, songNumber)
    if Random.OneIn(8) then
        return nil
    end

    local songIdx = Random.IntMax(#songList.Songs)
    return songList.Songs[songIdx+1]
end