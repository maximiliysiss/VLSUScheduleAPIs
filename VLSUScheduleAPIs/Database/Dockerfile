FROM microsoft/mssql-server-linux:2017-latest
ENV SA_PASSWORD=Pass@word
ENV ACCEPT_EULA=Y
COPY . .
RUN chmod +x ./SqlCmdStartup.sh
CMD /bin/bash ./entrypoint.sh