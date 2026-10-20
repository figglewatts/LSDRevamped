require "dreams"

player = GetEntity("__player")

interacted = false

function start()
    local big = IsDayLinear(3, 1) or (IsDayLinear(3, 0) and Random.OneIn(3))
    if big then
        this.GameObject.Scale = Unity.Vector3(6, 6, 6)
    end

    this.PlayAnimation(0)
    this.SetChildVisible(false)
    
    if not IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end
end

function intervalUpdate()
    if not interacted then return end

    this.LookAtPlane(player.WorldPosition)

    local playerDistance = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if playerDistance < 0.5 then
        interacted = false
        DreamSystem.SetNextTransitionDream(dreams.ViolenceDistrict)
        DreamSystem.TransitionToDream()
    end
end

function interact()
    this.SetChildVisible(true)
    interacted = true
    this.LogGraphContribution(3, -1)
end