function start()
    this.PlayAnimation(1)
    
    if not IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end

    if not activeOnDay(DreamSystem.DayNumber) then
        this.GameObject.SetActive(false)
        return
    end
end

function interact()
    this.LogGraphContribution(0, 5)
end

function activeOnDay(day)
    local zeroDay = ((day / 2) % 3) - 1
    return zeroDay == 0
end