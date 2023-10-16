require "dreams"

player = GetEntity("__player")

interacted = false

function start()
    if not IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end

    if Random.OneIn(4) then
        this.GameObject.Scale = Unity.Vector3(8, 8, 8)
    end

    this.PlayAnimation(0)
    this.SetChildVisible(false)
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