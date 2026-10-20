require "dreams"

player = GetEntity("__player")
audio = GetEntity("DogBarkAudio").DreamAudio

interacted = false
chasing = false
moveSpeed = 2

function start()
    this.SetChildVisible(false)
    
    if not IsDayEven() or not IsDayLinear(3, 1) then
        this.GameObject.SetActive(false)
        return
    end

    -- some are much quicker than others
    if Random.OneIn(2) then moveSpeed = 3.5 end
end

function intervalUpdate()
    if not chasing then return end

    local playerDistance = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if playerDistance < 0.4 then
        chasing = false
        DreamSystem.SetNextTransitionDream(dreams.SunFacesBridge)
        DreamSystem.TransitionToDream()
    end
end

function update()
    if not chasing then return end

    this.LookAtPlane(player.WorldPosition)
    this.MoveTowards(player.WorldPosition, moveSpeed)

    -- pressing the interact button sends it spinning away, and sometimes it gives up
    if Input.Interact.WasPressed then
        chasing = false
        this.Action
            .Do(|| this.Rotate(Unity.Vector3(0, 900, 0)))
            .Until(Condition.WaitForSeconds(0.4))
            .Then(function() chasing = Random.OneIn(2) end)
            .ThenFinish()
    end
end

function interact()
    this.SetChildVisible(true)
    this.PlayAnimation(0)
    this.LogGraphContribution(0, -1)
    audio.Play()
    interacted = true
    chasing = true
end