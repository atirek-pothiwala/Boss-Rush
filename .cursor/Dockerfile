FROM unityci/editor:ubuntu-6000.4.3f1-linux-il2cpp-3.2.2

RUN apt-get update \
    && apt-get install -y --no-install-recommends git-lfs \
    && git lfs install \
    && rm -rf /var/lib/apt/lists/*

ENV UNITY_PATH=/opt/unity/Editor/Unity
