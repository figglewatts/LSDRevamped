require "dreams"

player = GetEntity("__player")

function start()
    if IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end
end

function intervalUpdate()
    if not interacted then return end

    local playerDistance = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if playerDistance < 2 then
        this.SetChildVisible(false)
    else
        this.SetChildVisible(true)
    end
end

function interact()
    interacted = true
    this.LogGraphContribution(0, 8)
end