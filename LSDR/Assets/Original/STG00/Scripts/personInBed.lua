require "dreams"

bedPerson = GetEntity("BedPerson")
bedDead = GetEntity("BedDead")
bedEmpty = GetEntity("BedEmpty")
player = GetEntity("__player")

state = 0
linked = false

function start()
    if not IsDayEven() then
        -- this.GameObject.SetActive(false)
        -- return
    end

    local dayNumber = DreamSystem.DayNumber
    if dayNumber < 10 then
        state = 0
        bedDead.SetActive(false)
        bedEmpty.SetActive(false)
    elseif dayNumber < 20 then
        state = 1
        bedPerson.SetActive(false)
        bedEmpty.SetActive(false)
    else
        state = 2
        bedPerson.SetActive(false)
        bedDead.SetActive(false)
    end
end

function update()
    if linked then return end

    local playerDist = (player.WorldPosition - this.GameObject.WorldPosition).length()
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
        DreamSystem.LogGraphContributionFromEntity(0, 0)
    elseif state == 1 then
        DreamSystem.LogGraphContributionFromEntity(0, -3)
    else
        DreamSystem.LogGraphContributionFromEntity(0, -7)
    end
end