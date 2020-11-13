function start()
    audioClip = Resources.LoadAudio("sfx/SE_00001.ogg")

    turnAnim = obj.GetAnimation(0)
    obj.Action
        .Do(|| obj.PlayAnimation(1))
        .Then(|| obj.Translate(Vector3(0, 0, 1) * DeltaTime()))
            .Until(Condition.WaitForSeconds(1.5))
        .Then(|| obj.RotateAngleAxis(180, obj.Up))
        .Then(|| obj.PlayAnimation(0))
        .Then(|| obj.PlayAudio(audioClip))
        .Then(nil)
            .Until(Condition.WaitForAnimation(turnAnim))
        .Then(|| obj.PlayAnimation(1))
        .Then(|| obj.Translate(Vector3(0, 0, 1) * DeltaTime()))
            .Until(Condition.WaitForSeconds(1.5))
        .Then(|| obj.RotateAngleAxis(180, obj.Up))
        .Then(|| obj.PlayAnimation(0))
        .Then(|| obj.PlayAudio(audioClip))
        .Then(nil)
            .Until(Condition.WaitForAnimation(turnAnim))
        .ThenLoop()
end

function update()
end











