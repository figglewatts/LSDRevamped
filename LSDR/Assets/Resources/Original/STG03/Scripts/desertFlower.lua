require "dreams"

player = GetEntity("__player")

interacted = false

function start()
    if Random.OneIn(2) then
        this.GameObject.Scale = Unity.Vector3(0.5, 0.5, 0.5)
    end
end

function intervalUpdate()
    if not interacted then return end
    local distanceToPlayer = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if distanceToPlayer < 0.5 then
        interacted = false
        DreamSystem.SetNextTransitionDream(dreams.Happytown)
        DreamSystem.TransitionToDream()
    end
end

function interact()
    this.LogGraphContribution(6, 2)
    interacted = true

    this.Action
        .Do(|| this.PlayAnimation(0))
        .ThenWaitUntil(this.WaitForAnimation(0, -0.25))
        .Then(|| this.StopAnimation())
        .ThenFinish()
end