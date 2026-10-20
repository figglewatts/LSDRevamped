audio = GetEntity(this.GameObject.Name .. "Audio").DreamAudio
trainEntity = GetEntity("TrainKyoto")

function start()
    trainEntity.SetActive(false)

    if not IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end
end

function interact()
    audio.Play()

    this.Action
        .WaitUntil(Condition.WaitForSeconds(20))
        .Then(|| trainEntity.SetActive(true))
        .Then(|| audio.Stop())
        .ThenFinish()
end