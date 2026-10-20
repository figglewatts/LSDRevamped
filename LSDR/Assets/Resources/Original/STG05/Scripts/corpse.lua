require "dreams"

player = GetEntity("__player")
linked = false

function start()
    local pileUp = tonumber(string.sub(this.GameObject.Name, 7)) > 3
    if IsDayEven() or (pileUp and not IsDayLinear(3, 1)) then
        this.GameObject.SetActive(false)
        return
    end

    this.GameObject.WorldPosition = this.GameObject.WorldPosition + Unity.Vector3(0, 0.1, 0)
    DreamSystem.ArmTeleports()
end

function intervalUpdate()
    if linked then return end

    local playerDistance = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if playerDistance < 0.5 then
        linked = true
        DreamSystem.SetNextTransitionDream(dreams.ViolenceDistrict)
        DreamSystem.TransitionToDream()
    end
end

function interact()
    this.LogGraphContribution(0, -4)
end