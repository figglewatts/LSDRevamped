player = GetEntity("__player")

interacted = false

function start()
    if IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end
end

function intervalUpdate()
    if not interacted then return end

    local playerDistance = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if playerDistance < 1 then
        interacted = false
        DreamSystem.EndDream() -- TODO: play the gears video here
    end
end

function interact()
    this.LogGraphContribution(-6, 7)
end