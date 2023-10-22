require "dreams"

audio = GetEntity(this.GameObject.Name .. "Audio").DreamAudio
player = GetEntity("__player")

distanceToPlayer = 0
interacted = false

function start()
    if not IsDayEven() then
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

    if distanceToPlayer < 0.4 and not linked then
        linked = true
        DreamSystem.SetNextTransitionDream(dreams.Kyoto)
        DreamSystem.TransitionToDream()
    end
end

function interact()
    interacted = true

    local target = player.WorldPosition
    target.y = this.GameObject.WorldPosition.y
    this.LookAt(target)

    this.Action
        .Do(|| this.PlayAnimation(0))
        .ThenWaitUntil(Condition.WaitForSeconds(1.5))
        .Then(|| audio.Play())
        .ThenWaitUntil(Condition.WaitForSeconds(2.5))
        .Then(|| this.StopAnimation())
        .Then(|| this.MoveInDirection(this.Up, 1))
        .Until(Condition.WaitForSeconds(10))
        .Then(|| this.GameObject.SetActive(false))
        .ThenFinish()
end