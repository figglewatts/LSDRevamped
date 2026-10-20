require "dreams"

player = GetEntity("__player")
distanceToPlayer = 0

function start()
    if this.GameObject.Name ~= "Peacock1" and not IsDayLinear(3, 1) then
        this.GameObject.SetActive(false)
        return
    end
end

function intervalUpdate()
    distanceToPlayer = (player.WorldPosition - this.GameObject.WorldPosition).length()
    this.LookAtPlane(player.WorldPosition)
end

function update()
    if distanceToPlayer < 0.4 and not linked then
        linked = true
        DreamSystem.SetNextTransitionDream(dreams.Happytown)
        DreamSystem.TransitionToDream()
    end
end

function interact()
    this.PlayAnimation(0)
    this.LogGraphContribution(1, 4)
end