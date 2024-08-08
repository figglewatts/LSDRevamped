require "dreams"

bedPerson = GetEntity("BedPerson")
bedDead = GetEntity("BedDead")
bedEmpty = GetEntity("BedEmpty")
player = GetEntity("__player")

state = 0
linked = false
playerDist = 0

function start()
    local dayNumber = DreamSystem.DayNumber % 90
    if dayNumber < 30 then
        state = 0
        bedDead.SetActive(false)
        bedEmpty.SetActive(false)
    elseif dayNumber < 60 then
        state = 1
        bedPerson.SetActive(false)
        bedEmpty.SetActive(false)
    else
        state = 2
        bedPerson.SetActive(false)
        bedDead.SetActive(false)
    end
    
    if not IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end
end

function intervalUpdate()
    playerDist = (player.WorldPosition - this.GameObject.WorldPosition).length()
end

function update()
    if linked then return end

    if playerDist < 0.4 then
        linked = true

        if state == 0 then
            DreamSystem.SetNextTransitionDream(dreams.Happytown)
        elseif state == 1 then
            DreamSystem.SetNextTransitionDream(dreams.ViolenceDistrict)
        else
            DreamSystem.SetNextTransitionDream(dreams.Void)
        end
        DreamSystem.TransitionToDream()
    end
end

function interact()
    if state == 0 then
        this.LogGraphContribution(0, 0)
    elseif state == 1 then
        this.LogGraphContribution(0, -3)
    else
        this.LogGraphContribution(0, -7)
    end
end