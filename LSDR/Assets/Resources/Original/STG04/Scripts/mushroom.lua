require "dreams"

player = GetEntity("__player")

interacted = false

function start()
    if Random.OneIn(3) then
        this.GameObject.Scale = Unity.Vector3(1, 2, 1)
    end
    
    if not IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end
end

function intervalUpdate()
    if not interacted then return end

    local playerDistance = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if playerDistance < 0.3 then
        interacted = false
        DreamSystem.SetNextTransitionDream(dreams.Happytown)
        DreamSystem.TransitionToDream()
    end
end

function interact()
    this.LogGraphContribution(-1, -1)
    this.PlayAnimation(0)
    interacted = true
end
