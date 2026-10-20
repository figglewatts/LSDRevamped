require "dreams"

audio = GetEntity(this.GameObject.Name .. "Audio").DreamAudio
player = GetEntity("__player")

distanceToPlayer = 0
interacted = false

function start()
    if not IsDayEven() or (this.GameObject.Name == "SecretBaby" and not IsDayLinear(3, 1)) then
        this.GameObject.SetActive(false)
        return
    end
end

function intervalUpdate()
    if not interacted then return end
    distanceToPlayer = (player.WorldPosition - this.GameObject.WorldPosition).length()
end

function update()
    if not interacted then return end

    this.LookAtPlane(player.WorldPosition)

    if distanceToPlayer < 0.4 and not linked then
        linked = true
        DreamSystem.SetNextTransitionDream(dreams.Kyoto)
        DreamSystem.TransitionToDream()
    end
end

function interact()
    interacted = true

    this.Action
        .Do(|| this.PlayAnimation(0))
        .ThenWaitUntil(Condition.WaitForSeconds(1))
        .Then(|| audio.Play())
        .ThenWaitUntil(this.WaitForAnimation(0, -1))
        .Then(|| this.StopAnimation())
        .Then(|| this.MoveInDirection(this.Up, 5))
        .Until(Condition.WaitForSeconds(3))
        .Then(|| this.GameObject.SetActive(false))
        .ThenFinish()
end