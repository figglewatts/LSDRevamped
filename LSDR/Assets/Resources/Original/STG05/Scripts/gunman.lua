require "dreams"

victim = GetEntity("GunmanVictim").AnimatedObject
audio = GetEntity(this.GameObject.Name .. "Audio").DreamAudio
player = GetEntity("__player")

interacted = false
moveSpeed = 0.1
state = "waiting"
stateTime = 0
pressed = false
faded = false

inRow = { Gunman3 = true, Gunman4 = true, Gunman5 = true, Gunman6 = true }

function start()
    -- a row of them turns up in the cyan part of the chart
    if not IsDayEven() or (inRow[this.GameObject.Name] and ChartZone() ~= "cyan") then
        this.GameObject.SetActive(false)
        return
    end
end

function intervalUpdate()
    if not interacted then return end

    local playerDistance = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if playerDistance < 0.75 then
        interacted = false
        DreamSystem.SetNextTransitionDream(dreams.ClockworkMachines)
        DreamSystem.TransitionToDream()
    end
end

function update()
    if state == "waiting" then return end

    stateTime = stateTime + Unity.DeltaTime()

    if state == "aiming" then
        if stateTime > this.GetAnimationLengthSeconds(0) then
            this.StopAnimation()
            setState("turning")
        end
    elseif state == "turning" then
        -- it turns about for a moment, and pressing the button now sends it rolling away
        this.Rotate(Unity.Vector3(0, 180, 0))
        if Input.Interact.WasPressed then pressed = true end
        if stateTime > 0.5 then
            if pressed then setState("rolling") else shoot() end
        end
    elseif state == "rolling" then
        this.Rotate(Unity.Vector3(0, 0, 360))
        this.MoveInDirection(this.Forward.negated(), 2)
        if stateTime > 0.5 then setState("done") end
    elseif state == "approaching" then
        this.LookAtPlane(player.WorldPosition)
        this.MoveTowards(player.WorldPosition, moveSpeed)
        if stateTime > 1.5 then
            DreamSystem.Flash(Colors.White, 1.5)
            setState("shot")
        end
    elseif state == "shot" then
        -- the view rolls over and goes dark
        player.LocalRotation = player.LocalRotation + Unity.Vector3(0, 0, 20 * Unity.DeltaTime())
        if stateTime > 1.5 and not faded then
            faded = true
            DreamSystem.FadeToBlack(1.25)
        end
        if stateTime > 4.5 then
            setState("done")
            local rotation = player.LocalRotation
            player.LocalRotation = Unity.Vector3(rotation.x, rotation.y + 180, 0)
            if Random.OneIn(5) then
                DreamSystem.EndDream()
            else
                DreamSystem.TransitionToDream()
            end
        end
    end
end

function setState(newState)
    state = newState
    stateTime = 0
end

function shoot()
    interacted = true
    SetCanControlPlayer(false)
    setState("approaching")
end

function interact()
    audio.Play()
    this.LogGraphContribution(3, 0)

    if this.GameObject.Name == "GunmanWithVictim" then
        -- shoot the victim first, then turn on the player
        this.Action
            .Do(|| victim.Play(0))
            .Then(|| this.LogGraphContribution(0, -8))
            .ThenWaitUntil(Condition.WaitForSeconds(0.75))
            .Then(|| victim.Stop())
            .ThenWaitUntil(Condition.WaitForSeconds(4))
            .Then(aim)
            .ThenFinish()
    else
        aim()
    end
end

function aim()
    this.PlayAnimation(0)
    setState("aiming")
end